/*——- Omar Hussein ——-*/
// =========================================================
// Accounts module — سلوك الصفحات: OTP، العداد، قوة الباسورد، خطوات تسجيل الدكتور،
// رفع الملفات، الكروت (radio)، فتح المودال بعد خطأ، ورسائل الـ Flash
// =========================================================
(function () {
    "use strict";

    // ---------- Checkbox لازم يتعلّم عليه: [MustBeTrue] ----------
    if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
        jQuery.validator.addMethod("mustbetrue", (value, element) => element.checked);
        jQuery.validator.unobtrusive.adapters.addBool("mustbetrue");
    }

    const $$ = (sel, root) => Array.from((root || document).querySelectorAll(sel));
    const refreshIcons = () => window.lucide && window.lucide.createIcons();

    // ---------- OTP: 6 خانات بتملا input مخفي ----------
    $$("[data-otp]").forEach(box => {
        const inputs = $$("input[data-otp-digit]", box);
        const hidden = box.parentElement.querySelector("input[data-otp-value]");
        const sync = () => { if (hidden) hidden.value = inputs.map(i => i.value).join(""); };

        inputs.forEach((input, index) => {
            input.addEventListener("input", () => {
                input.value = input.value.replace(/\D/g, "").slice(-1);
                if (input.value && inputs[index + 1]) inputs[index + 1].focus();
                sync();
            });
            input.addEventListener("keydown", e => {
                if (e.key === "Backspace" && !input.value && inputs[index - 1]) inputs[index - 1].focus();
            });
            input.addEventListener("paste", e => {
                const digits = (e.clipboardData.getData("text") || "").replace(/\D/g, "").slice(0, inputs.length);
                if (!digits) return;
                e.preventDefault();
                digits.split("").forEach((d, i) => { inputs[i].value = d; });
                (inputs[digits.length] || inputs[inputs.length - 1]).focus();
                sync();
            });
        });
        if (inputs[0]) inputs[0].focus();
    });

    // ---------- عداد "إعادة الإرسال بعد 0:45" ----------
    $$("[data-countdown]").forEach(el => {
        let seconds = parseInt(el.getAttribute("data-countdown"), 10) || 0;
        const button = document.getElementById(el.getAttribute("data-countdown-target"));
        const label = el.querySelector("[data-countdown-time]");
        const tick = () => {
            if (seconds <= 0) {
                el.hidden = true;
                if (button) { button.hidden = false; button.disabled = false; }
                return;
            }
            if (label) label.textContent = Math.floor(seconds / 60) + ":" + String(seconds % 60).padStart(2, "0");
            seconds--;
            setTimeout(tick, 1000);
        };
        if (button && seconds > 0) button.hidden = true;
        tick();
    });

    // ---------- قوة الباسورد ----------
    $$("input[data-strength]").forEach(input => {
        const meter = document.getElementById(input.getAttribute("data-strength"));
        if (!meter) return;
        const bars = meter.querySelector(".mc-strength");
        const label = meter.querySelector(".mc-strength-label");
        const rules = {
            length: v => v.length >= 8,
            case: v => /[a-z]/.test(v) && /[A-Z]/.test(v),
            number: v => /\d/.test(v)
        };
        const update = () => {
            const v = input.value;
            let score = 0;
            Object.keys(rules).forEach(name => {
                const ok = rules[name](v);
                if (ok) score++;
                const li = meter.querySelector('[data-rule="' + name + '"]');
                if (li) li.classList.toggle("is-ok", ok);
            });
            const level = !v ? 0 : score === 3 ? (v.length >= 12 ? 4 : 3) : Math.max(1, score);
            bars.setAttribute("data-level", level);
            if (label) {
                label.textContent = level === 0 ? "" : level >= 3 ? label.dataset.strong : label.dataset.weak;
                label.style.color = level >= 3 ? "var(--success-500)" : "var(--neutral-600)";
            }
        };
        input.addEventListener("input", update);
        update();
    });

    // ---------- كروت الاختيار (radio) ----------
    const syncChoices = name => $$('input[type="radio"][name="' + name + '"]').forEach(r => {
        const card = r.closest(".mc-choice");
        if (card) card.classList.toggle("is-selected", r.checked);
    });
    $$(".mc-choice input[type=radio]").forEach(radio => {
        radio.addEventListener("change", () => syncChoices(radio.name));
        syncChoices(radio.name);
    });

    // ---------- رفع الملفات: اسم الملف + معاينة الصورة ----------
    $$("input[type=file][data-file-label]").forEach(input => {
        const label = document.getElementById(input.getAttribute("data-file-label"));
        const box = input.closest(".mc-upload");
        const preview = document.getElementById(input.getAttribute("data-preview") || "");
        const original = label ? label.textContent : "";
        input.addEventListener("change", () => {
            const files = Array.from(input.files || []);
            if (label) label.textContent = files.length ? files.map(f => f.name).join(", ") : original;
            if (box) box.classList.toggle("is-filled", files.length > 0);
            if (preview && files[0] && files[0].type.startsWith("image/")) {
                const reader = new FileReader();
                reader.onload = e => {
                    preview.style.backgroundImage = "url('" + e.target.result + "')";
                    preview.classList.add("has-image");
                };
                reader.readAsDataURL(files[0]);
            }
            if (window.jQuery && jQuery.validator && input.form) jQuery(input).valid();
        });
    });

    // ---------- خطوات تسجيل الدكتور (فورم واحدة بـ 3 خطوات) ----------
    $$("form[data-steps]").forEach(form => {
        const panels = $$(".mc-step-panel", form);
        const indicators = $$("[data-step-indicator]", form.parentElement);
        let current = 0;

        const show = index => {
            current = index;
            panels.forEach((p, i) => { p.hidden = i !== index; });
            indicators.forEach((el, i) => {
                el.classList.toggle("is-active", i === index);
                el.classList.toggle("is-done", i < index);
                const dot = el.querySelector(".mc-steps__dot");
                if (dot) dot.innerHTML = i < index ? '<i data-lucide="check" class="mc-icon-sm" style="width:12px;height:12px"></i>' : String(i + 1);
            });
            refreshIcons();
            window.scrollTo({ top: 0, behavior: "smooth" });
        };

        const panelValid = panel => {
            if (!(window.jQuery && jQuery.validator)) return true;
            const validator = jQuery(form).validate();
            let ok = true;
            $$("input, select, textarea", panel).forEach(el => {
                if (el.name && !validator.element(el)) ok = false;
            });
            return ok;
        };

        $$("[data-step-next]", form).forEach(btn => btn.addEventListener("click", () => {
            if (panelValid(panels[current])) show(Math.min(current + 1, panels.length - 1));
        }));
        $$("[data-step-back]", form).forEach(btn => btn.addEventListener("click", () => show(Math.max(current - 1, 0))));

        // بعد خطأ من السيرفر: نفتح أول خطوة فيها خطأ
        const firstError = panels.findIndex(p => p.querySelector(".field-validation-error, .input-validation-error"));
        const summaryHasErrors = form.querySelector(".validation-summary-errors");
        show(firstError >= 0 ? firstError : summaryHasErrors ? panels.length - 1 : 0);
    });

    // ---------- "لسه شغال هنا" بيقفل سنة النهاية ----------
    $$("input[data-disables]").forEach(cb => {
        const target = document.getElementById(cb.getAttribute("data-disables"));
        const sync = () => { if (target) { target.disabled = cb.checked; if (cb.checked) target.value = ""; } };
        cb.addEventListener("change", sync);
        sync();
    });

    // ---------- مودال بيملا قيم من الزرار (طلب تعديل / رفع مستند / رفض) ----------
    document.addEventListener("click", e => {
        const trigger = e.target.closest("[data-modal-fill]");
        if (!trigger) return;
        const modal = document.querySelector(trigger.getAttribute("data-bs-target"));
        if (!modal) return;
        const data = JSON.parse(trigger.getAttribute("data-modal-fill"));
        Object.keys(data).forEach(key => {
            $$('[data-fill="' + key + '"]', modal).forEach(el => {
                if ("value" in el && el.tagName !== "DIV" && el.tagName !== "SPAN") el.value = data[key];
                else el.textContent = data[key];
            });
        });
        $$("[data-fill-show]", modal).forEach(el => {
            el.hidden = el.getAttribute("data-fill-show") !== String(data.mode || "");
        });
    });

    // ---------- فتح المودال تلقائي لو الصفحة رجعت بخطأ ----------
    const auto = document.querySelector("[data-open-modal]");
    if (auto && window.bootstrap) {
        const modal = document.getElementById(auto.getAttribute("data-open-modal"));
        if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
    }

    // ---------- تأكيد قبل الإجراءات المهمة ----------
    document.addEventListener("submit", e => {
        const msg = e.target.getAttribute("data-confirm");
        if (msg && !window.confirm(msg)) e.preventDefault();
    });

    // ---------- Flash ----------
    $$(".mc-flash").forEach(el => {
        const close = () => el.remove();
        const btn = el.querySelector("button");
        if (btn) btn.addEventListener("click", close);
        setTimeout(close, 6000);
    });
})();
/*————*/
