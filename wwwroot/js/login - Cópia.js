        const wrapper = document.querySelector('.wrapper');
        const loginLink = document.querySelector('.login-link');
        const registerLink = document.querySelector('.register-link');

        // Seleciona os formulários
        const loginForm = wrapper.querySelector('.form-box.login form');
        const registerForm = wrapper.querySelector('.form-box.register form');

        // Limpa os campos e reseta os labels
        function resetForm(form) {
            form.reset();
            const inputs = form.querySelectorAll('input');
            inputs.forEach(input => {
                input.dispatchEvent(new Event('input'));
            });
        }

        // Ao clicar em "Cadastre-se aqui" (ativa o formulário de cadastro)
        registerLink.addEventListener('click', (e) => {
            e.preventDefault();
            wrapper.classList.add('active');
            resetForm(loginForm);
        });

        // Ao clicar em "Entra" (volta para o login)
        const loginLinks = document.querySelectorAll('.login-link');
        loginLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                wrapper.classList.remove('active');
                resetForm(registerForm);
            });
        });

        // Funcionalidade de mostrar/ocultar senha
        document.querySelectorAll('.toggle-password').forEach(icon => {
            icon.addEventListener('click', function() {
                const input = this.closest('.input-box').querySelector('.password-input');
                const isPassword = input.type === 'password';
                
                input.type = isPassword ? 'text' : 'password';
                this.textContent = isPassword ? 'visibility_off' : 'visibility';
            });
        });

