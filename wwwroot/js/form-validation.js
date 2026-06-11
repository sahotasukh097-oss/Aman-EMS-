// Form Validation and Handling
(function() {
    'use strict';

    document.addEventListener('DOMContentLoaded', function() {
        initializeFormValidation();
    });

    function initializeFormValidation() {
        // Bootstrap form validation
        const forms = document.querySelectorAll('.needs-validation');
        Array.from(forms).forEach(form => {
            form.addEventListener('submit', event => {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });

        // Custom email validation
        const emailInputs = document.querySelectorAll('input[type="email"]');
        emailInputs.forEach(input => {
            input.addEventListener('blur', validateEmail);
        });

        // Password strength indicator
        const passwordInputs = document.querySelectorAll('input[type="password"]');
        passwordInputs.forEach(input => {
            input.addEventListener('input', showPasswordStrength);
        });

        // Form submit handlers
        setupFormSubmitHandlers();
    }

    function validateEmail(event) {
        const email = event.target.value;
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        
        if (email && !emailRegex.test(email)) {
            event.target.classList.add('is-invalid');
            showValidationError(event.target, 'Please enter a valid email address');
        } else {
            event.target.classList.remove('is-invalid');
            removeValidationError(event.target);
        }
    }

    function showPasswordStrength(event) {
        const password = event.target.value;
        let strength = 0;
        let feedback = '';

        // Check password strength
        if (password.length >= 8) strength++;
        if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
        if (/[0-9]/.test(password)) strength++;
        if (/[^a-zA-Z0-9]/.test(password)) strength++;

        // Provide feedback
        const strengthContainer = event.target.parentElement.querySelector('.password-strength');
        if (strengthContainer) {
            strengthContainer.remove();
        }

        const strengthBar = document.createElement('div');
        strengthBar.className = 'password-strength mt-2';
        strengthBar.style.height = '5px';
        strengthBar.style.borderRadius = '3px';
        strengthBar.style.marginTop = '5px';

        switch(strength) {
            case 0:
            case 1:
                strengthBar.style.backgroundColor = '#dc3545';
                feedback = 'Weak';
                break;
            case 2:
                strengthBar.style.backgroundColor = '#ffc107';
                feedback = 'Fair';
                break;
            case 3:
                strengthBar.style.backgroundColor = '#17a2b8';
                feedback = 'Good';
                break;
            case 4:
                strengthBar.style.backgroundColor = '#28a745';
                feedback = 'Strong';
                break;
        }

        strengthBar.setAttribute('data-feedback', feedback);
        event.target.parentElement.appendChild(strengthBar);
    }

    function showValidationError(element, message) {
        let errorSpan = element.nextElementSibling;
        if (!errorSpan || !errorSpan.classList.contains('invalid-feedback')) {
            errorSpan = document.createElement('span');
            errorSpan.className = 'invalid-feedback d-block';
            element.parentElement.appendChild(errorSpan);
        }
        errorSpan.textContent = message;
    }

    function removeValidationError(element) {
        const errorSpan = element.nextElementSibling;
        if (errorSpan && errorSpan.classList.contains('invalid-feedback')) {
            errorSpan.remove();
        }
    }

    function setupFormSubmitHandlers() {
        // Login form
        const loginForm = document.querySelector('.login-form');
        if (loginForm) {
            loginForm.addEventListener('submit', handleLoginSubmit);
        }

        // Signup form
        const signupForm = document.querySelector('.signup-form');
        if (signupForm) {
            signupForm.addEventListener('submit', handleSignupSubmit);
        }
    }

    function handleLoginSubmit(event) {
        // Additional client-side validation if needed
        const email = this.querySelector('input[type="email"]').value;
        const password = this.querySelector('input[type="password"]').value;

        if (!email || !password) {
            event.preventDefault();
            AppUtils.showNotification('Please fill in all fields', 'warning');
        }
    }

    function handleSignupSubmit(event) {
        const password = this.querySelector('input[name="Password"]');
        const confirmPassword = this.querySelector('input[name="ConfirmPassword"]');

        if (password && confirmPassword && password.value !== confirmPassword.value) {
            event.preventDefault();
            AppUtils.showNotification('Passwords do not match', 'danger');
        }
    }

    // Expose functions globally
    window.FormValidation = {
        validateEmail: validateEmail,
        showPasswordStrength: showPasswordStrength
    };
})();
