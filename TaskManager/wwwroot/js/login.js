function resetRegisterSteps() {
    const steps = document.querySelectorAll('.auth-form.register-form .form-step');
    if (steps.length > 0) {
        steps.forEach((step, index) => {
            step.classList.toggle('active', index === 0);
        });
    }
}

function initTabs() {
    const tabs = document.querySelectorAll('.auth-tab');
    const forms = document.querySelectorAll('.auth-form');
    const indicator = document.querySelector('.active-indicator');

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const tabName = tab.dataset.tab;

            tabs.forEach(t => {
                t.classList.toggle('active', t === tab);
                t.setAttribute('aria-selected', t === tab ? 'true' : 'false');
            });

            forms.forEach(form => {
                const isActive = (tabName === 'login' && form.action.includes('SignIn')) ||
                    (tabName === 'register' && form.action.includes('Register'));
                form.classList.toggle('active', isActive);
            });

            indicator.style.transform = tabName === 'login' ? 'translateX(0)' : 'translateX(100%)';

            if (tabName === 'register') {
                resetRegisterSteps();
            }
        });
    });
}

function initStepButtons() {
    document.querySelectorAll('.btn-next-step').forEach(button => {
        button.addEventListener('click', function () {
            const currentStep = this.closest('.form-step');
            const nextStep = currentStep.nextElementSibling;

            if (nextStep) {
                currentStep.classList.remove('active');
                nextStep.classList.add('active');
            }
        });
    });

    document.querySelectorAll('.btn-prev-step').forEach(button => {
        button.addEventListener('click', function () {
            const currentStep = this.closest('.form-step');
            const prevStep = currentStep.previousElementSibling;

            if (prevStep) {
                currentStep.classList.remove('active');
                prevStep.classList.add('active');
            }
        });
    });
}

document.addEventListener('DOMContentLoaded', function () {
    initTabs();
    initStepButtons();
});