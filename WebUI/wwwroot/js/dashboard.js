// ==================== Dashboard JavaScript ====================

/**
 * Sidebar toggle functionality
 */
const sidebarToggle = document.getElementById('sidebarToggle');
const sidebar = document.querySelector('.dashboard-sidebar');

if (sidebarToggle && sidebar) {
    sidebarToggle.addEventListener('click', function () {
        sidebar.classList.toggle('active');
    });

    // Close sidebar when clicking on a nav link (mobile)
    const navLinks = sidebar.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('active');
            }
        });
    });
}

/**
 * Show coming soon message
 */
function showComingSoon(featureName) {
    if (typeof AlertModal !== 'undefined' && AlertModal.bootstrapModal) {
        AlertModal.info(`"${featureName}" özelliği yakında kullanıma açılacaktır.`);
    } else {
        alert(`"${featureName}" özelliği yakında kullanıma açılacaktır.`);
    }
}

/**
 * Sidebar responsive behavior
 */
function handleResponsive() {
    if (window.innerWidth > 768) {
        sidebar?.classList.remove('active');
    }
}

window.addEventListener('resize', handleResponsive);

/**
 * Smooth animations on load
 */
document.addEventListener('DOMContentLoaded', function () {
    // Animate stat cards on load
    const statCards = document.querySelectorAll('.stat-card');
    statCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Animate welcome card
    const welcomeCard = document.querySelector('.welcome-card');
    if (welcomeCard) {
        welcomeCard.style.opacity = '0';
        welcomeCard.style.transform = 'translateX(-20px)';
        setTimeout(() => {
            welcomeCard.style.transition = 'all 0.6s ease';
            welcomeCard.style.opacity = '1';
            welcomeCard.style.transform = 'translateX(0)';
        }, 50);
    }

    // Set active nav item based on current page
    const currentPage = window.location.pathname.toLowerCase();
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(item => {
        item.classList.remove('active');
    });

    // Add active class to Dashboard nav item
    if (currentPage.includes('dashboard')) {
        navItems.forEach(item => {
            if (item.querySelector('.nav-link').href.includes('dashboard')) {
                item.classList.add('active');
            }
        });
    }
});

/**
 * Logout confirmation
 */
const logoutLink = document.querySelector('a[href*="Logout"]');
if (logoutLink) {
    logoutLink.addEventListener('click', function (e) {
        if (typeof AlertModal !== 'undefined' && AlertModal.bootstrapModal) {
            e.preventDefault();
            AlertModal.confirm(
                'Çıkış yapmak istediğinizden emin misiniz?',
                'Çıkış Yapılsın mı?',
                function (confirmed) {
                    if (confirmed) {
                        window.location.href = logoutLink.href;
                    }
                }
            );
        }
    });
}

/**
 * Auto-hide sidebar on mobile when page loads
 */
if (window.innerWidth <= 768) {
    sidebar?.classList.remove('active');
}
