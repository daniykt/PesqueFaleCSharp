// Toast notification functionality
(function () {
    const toast = document.getElementById('siteToast');
    if (!toast) return;

    let dismissTimer;
    let progressPaused = false;

    // Function to show toast
    function showToast() {
        setTimeout(function () {
            toast.classList.add('show');
        }, 50);

        // Auto-dismiss após 10s (alterado de 5000 para 10000)
        dismissTimer = setTimeout(hideToast, 10000);

        // Pausar progresso ao passar mouse
        toast.addEventListener('mouseenter', pauseProgress);
        toast.addEventListener('mouseleave', resumeProgress);
    }

    // Function to hide toast
    function hideToast() {
        if (!toast) return;
        clearTimeout(dismissTimer);
        toast.classList.remove('show');

        // Remove do DOM após animação
        setTimeout(function () {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }

    // Pausar a animação de progresso
    function pauseProgress() {
        progressPaused = true;
        const progressBar = toast.style.animationPlayState;
        toast.style.animationPlayState = 'paused';
        clearTimeout(dismissTimer);
    }

    // Retomar a animação de progresso
    function resumeProgress() {
        if (!progressPaused) return;

        toast.style.animationPlayState = 'running';
        const remainingTime = calculateRemainingTime();
        dismissTimer = setTimeout(hideToast, remainingTime);
        progressPaused = false;
    }

    // Calcular tempo restante baseado na animação CSS
    function calculateRemainingTime() {
        const computedStyle = window.getComputedStyle(toast, '::before');
        const transform = computedStyle.transform;
        const matrix = new DOMMatrixReadOnly(transform);
        const scaleX = matrix.m11;

        // 10000ms total (10 segundos), calcular baseado no progresso atual
        return scaleX * 10000;
    }

    // Adicionar event listener para o botão fechar
    const closeBtn = toast.querySelector('.toast-close-btn');
    if (closeBtn) {
        closeBtn.addEventListener('click', hideToast);
    }

    // Também fechar ao clicar no toast (exceto no botão fechar)
    toast.addEventListener('click', function (e) {
        if (e.target === toast || e.target.closest('.toast-content')) {
            hideToast();
        }
    });

    // Inicializar
    showToast();

    // Expor função global para fechar manualmente se necessário
    window.hideToast = hideToast;

})();