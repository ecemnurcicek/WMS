// Entry Area JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Toast elements
    const successToast = new bootstrap.Toast(document.getElementById('successToast'));
    const errorToast = new bootstrap.Toast(document.getElementById('errorToast'));

    // Assign shelf buttons
    const assignButtons = document.querySelectorAll('.btn-assign-shelf');
    
    assignButtons.forEach(button => {
        button.addEventListener('click', async function() {
            const transferDetailId = this.getAttribute('data-transfer-detail-id');
            const productId = this.getAttribute('data-product-id');
            const quantity = this.getAttribute('data-quantity');
            
            // Find the corresponding select element
            const selectElement = document.querySelector(
                `.shelf-select[data-transfer-detail-id="${transferDetailId}"]`
            );
            
            if (!selectElement) {
                showError('Raf seçimi bulunamadı');
                return;
            }

            const shelfId = selectElement.value;

            if (!shelfId || shelfId === '') {
                showError('Lütfen bir raf seçin');
                selectElement.focus();
                return;
            }

            // Disable button
            this.disabled = true;
            const originalHtml = this.innerHTML;
            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

            try {
                const response = await fetch('/EntryArea/AssignShelf', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    body: new URLSearchParams({
                        transferDetailId: transferDetailId,
                        productId: productId,
                        shelfId: shelfId,
                        quantity: quantity
                    })
                });

                const result = await response.json();

                if (result.success) {
                    showSuccess(result.message || 'Raf ataması başarıyla yapıldı');
                    
                    // Remove the row from table after 1 second
                    setTimeout(() => {
                        const row = this.closest('tr');
                        row.style.transition = 'opacity 0.3s';
                        row.style.opacity = '0';
                        
                        setTimeout(() => {
                            row.remove();
                            
                            // Check if table is empty
                            const tbody = document.querySelector('.entry-area-table tbody');
                            if (tbody && tbody.children.length === 0) {
                                // Reload page to show empty state
                                location.reload();
                            } else {
                                // Update badge count
                                updateBadgeCount();
                            }
                        }, 300);
                    }, 1000);
                } else {
                    showError(result.message || 'Raf ataması yapılamadı');
                    this.disabled = false;
                    this.innerHTML = originalHtml;
                }
            } catch (error) {
                console.error('Error:', error);
                showError('Bir hata oluştu: ' + error.message);
                this.disabled = false;
                this.innerHTML = originalHtml;
            }
        });
    });

    // Helper functions
    function showSuccess(message) {
        document.getElementById('successMessage').textContent = message;
        successToast.show();
    }

    function showError(message) {
        document.getElementById('errorMessage').textContent = message;
        errorToast.show();
    }

    function updateBadgeCount() {
        const tbody = document.querySelector('.entry-area-table tbody');
        const count = tbody ? tbody.children.length : 0;
        const badge = document.querySelector('.header-info .badge');
        if (badge) {
            badge.innerHTML = `<i class="fas fa-box me-1"></i>Toplam ${count} Ürün Bekliyor`;
        }
    }

    // Keyboard shortcuts
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + R: Refresh page
        if ((e.ctrlKey || e.metaKey) && e.key === 'r') {
            // Allow default browser refresh
            return;
        }
    });

    // Copy functionality for EAN codes
    const eanCodes = document.querySelectorAll('td code');
    eanCodes.forEach(code => {
        code.style.cursor = 'pointer';
        code.title = 'Kopyalamak için tıklayın';
        
        code.addEventListener('click', function() {
            const text = this.textContent;
            navigator.clipboard.writeText(text).then(() => {
                // Visual feedback
                const originalText = this.textContent;
                this.textContent = '✓ Kopyalandı';
                this.style.color = '#28a745';
                
                setTimeout(() => {
                    this.textContent = originalText;
                    this.style.color = '';
                }, 1500);
            }).catch(err => {
                console.error('Kopyalama hatası:', err);
            });
        });
    });
});
