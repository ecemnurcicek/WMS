// Transfer Management JavaScript

let selectedProduct = null;
let transferDetails = [];

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeTransferManagement();
});

function initializeTransferManagement() {
    setupEventListeners();
    setupFilters();
    setupTabFilters();
    setupCopyIcons();
}

// Setup event listeners
function setupEventListeners() {
    // Add new transfer button
    const btnAddTransfer = document.getElementById('btnAddTransfer');
    if (btnAddTransfer) {
        btnAddTransfer.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Setup copy icons - event delegation for dynamically loaded content
function setupCopyIcons() {
    document.addEventListener('click', function(e) {
        if (e.target.classList.contains('copy-icon')) {
            const textToCopy = e.target.getAttribute('data-copy-text');
            if (textToCopy) {
                copyTransferCode(textToCopy, e.target);
            }
        }
    });
}

// Setup filters (Admin view)
function setupFilters() {
    const filterStatus = document.getElementById('filterStatus');
    const filterBrand = document.getElementById('filterBrand');
    const filterSearch = document.getElementById('filterSearch');

    if (filterStatus) {
        filterStatus.addEventListener('change', applyFilters);
    }
    if (filterBrand) {
        filterBrand.addEventListener('change', applyFilters);
    }
    if (filterSearch) {
        filterSearch.addEventListener('input', applyFilters);
    }
}

// Setup tab filters (Shop user view)
function setupTabFilters() {
    // Outgoing tab filters
    const outgoingStatus = document.getElementById('outgoingFilterStatus');
    const outgoingSearch = document.getElementById('outgoingFilterSearch');
    if (outgoingStatus) {
        outgoingStatus.addEventListener('change', function() {
            applyTabFilters('outgoingTable');
        });
    }
    if (outgoingSearch) {
        outgoingSearch.addEventListener('input', function() {
            applyTabFilters('outgoingTable');
        });
    }

    // Incoming tab filters
    const incomingStatus = document.getElementById('incomingFilterStatus');
    const incomingSearch = document.getElementById('incomingFilterSearch');
    if (incomingStatus) {
        incomingStatus.addEventListener('change', function() {
            applyTabFilters('incomingTable');
        });
    }
    if (incomingSearch) {
        incomingSearch.addEventListener('input', function() {
            applyTabFilters('incomingTable');
        });
    }

    // Sayfa açılışında filtreleri uygula (varsayılan "Bekliyor" seçili)
    if (outgoingStatus) applyTabFilters('outgoingTable');
    if (incomingStatus) applyTabFilters('incomingTable');
}

// Apply tab-specific filters
function applyTabFilters(tableId) {
    let statusFilter, searchFilter;
    
    if (tableId === 'outgoingTable') {
        statusFilter = document.getElementById('outgoingFilterStatus')?.value || '';
        searchFilter = document.getElementById('outgoingFilterSearch')?.value?.toLowerCase() || '';
    } else {
        statusFilter = document.getElementById('incomingFilterStatus')?.value || '';
        searchFilter = document.getElementById('incomingFilterSearch')?.value?.toLowerCase() || '';
    }

    const table = document.getElementById(tableId);
    if (!table) return;

    const rows = table.querySelectorAll('tbody tr');
    rows.forEach(row => {
        const status = row.dataset.status || '';
        const transferName = row.querySelector('.transfer-name')?.textContent?.toLowerCase() || '';

        let show = true;

        if (statusFilter && status !== statusFilter) {
            show = false;
        }

        if (searchFilter && !transferName.includes(searchFilter)) {
            show = false;
        }

        row.style.display = show ? '' : 'none';
    });
}

// Apply filters (Admin view)
function applyFilters() {
    const statusFilter = document.getElementById('filterStatus')?.value || '';
    const brandFilter = document.getElementById('filterBrand')?.value || '';
    const searchFilter = document.getElementById('filterSearch')?.value?.toLowerCase() || '';

    const rows = document.querySelectorAll('#transferTable tbody tr');
    
    rows.forEach(row => {
        const status = row.dataset.status || '';
        const fromBrand = row.dataset.fromBrand?.toLowerCase() || '';
        const toBrand = row.dataset.toBrand?.toLowerCase() || '';
        const transferName = row.querySelector('.transfer-name')?.textContent?.toLowerCase() || '';

        let show = true;

        if (statusFilter && status !== statusFilter) {
            show = false;
        }

        if (brandFilter) {
            const brandName = document.querySelector(`#filterBrand option[value="${brandFilter}"]`)?.textContent?.toLowerCase() || '';
            if (!fromBrand.includes(brandName) && !toBrand.includes(brandName)) {
                show = false;
            }
        }

        if (searchFilter && !transferName.includes(searchFilter)) {
            show = false;
        }

        row.style.display = show ? '' : 'none';
    });
}

// Open Create Modal
function openCreateModal() {
    transferDetails = [];
    selectedProduct = null;
    
    fetch('/Transfer/CreateForm')
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('formModalContainer');
            container.innerHTML = html;
            
            setupFormListeners();
            
            const modal = new bootstrap.Modal(document.getElementById('formModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Setup form listeners
function setupFormListeners() {
    const isAdminOrManager = document.getElementById('isAdminOrManager')?.value === 'true';
    
    // From brand change (Admin only)
    const fromBrand = document.getElementById('fromBrand');
    if (fromBrand && isAdminOrManager) {
        fromBrand.addEventListener('change', function() {
            loadShopsByBrand(this.value, 'fromShopId');
        });
    }
    
    // To brand change
    const toBrand = document.getElementById('toBrand');
    if (toBrand) {
        toBrand.addEventListener('change', function() {
            loadShopsByBrand(this.value, 'toShopId');
        });
    }
    
    // Product search
    const productSearch = document.getElementById('productSearch');
    if (productSearch) {
        let searchTimeout;
        productSearch.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            const term = this.value.trim();
            
            if (term.length < 2) {
                document.getElementById('productSearchResults').classList.remove('show');
                return;
            }
            
            searchTimeout = setTimeout(() => {
                searchProducts(term);
            }, 300);
        });
        
        // Hide results when clicking outside
        document.addEventListener('click', function(e) {
            if (!e.target.closest('#productSearch') && !e.target.closest('#productSearchResults')) {
                document.getElementById('productSearchResults')?.classList.remove('show');
            }
        });
    }
    
    // Add product button
    const btnAddProduct = document.getElementById('btnAddProduct');
    if (btnAddProduct) {
        btnAddProduct.addEventListener('click', addProductToTransfer);
    }
    
    // Non-admin: auto-load shops from same brand
    const userBrandId = document.getElementById('userBrandId')?.value;
    if (userBrandId && !isAdminOrManager) {
        loadShopsByBrand(userBrandId, 'toShopId');
    }

    // Form submit
    const form = document.getElementById('transferForm');
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            submitTransferForm();
        });
    }
}

// Load shops by brand
function loadShopsByBrand(brandId, selectId) {
    const select = document.getElementById(selectId);
    if (!select) return;
    
    if (!brandId) {
        select.innerHTML = '<option value="">-- Önce Marka Seçiniz --</option>';
        select.disabled = true;
        return;
    }
    
    select.innerHTML = '<option value="">-- Yükleniyor... --</option>';
    
    fetch(`/Transfer/GetShopsByBrand?brandId=${brandId}`)
        .then(response => response.json())
        .then(shops => {
            const userShopId = document.getElementById('currentUserShopId')?.value || 
                               document.getElementById('fromShopId')?.value || '';
            select.innerHTML = '<option value="">-- Mağaza Seçiniz --</option>';
            shops.forEach(shop => {
                // Kullanıcının kendi mağazasını listeden çıkar
                if (userShopId && shop.id.toString() === userShopId.toString()) return;
                const option = document.createElement('option');
                option.value = shop.id;
                option.textContent = shop.name;
                select.appendChild(option);
            });
            select.disabled = false;
        })
        .catch(error => {
            console.error('Error loading shops:', error);
            select.innerHTML = '<option value="">-- Yüklenemedi --</option>';
        });
}

// Search products
function searchProducts(term) {
    fetch(`/Transfer/SearchProducts?term=${encodeURIComponent(term)}`)
        .then(response => response.json())
        .then(products => {
            const resultsDiv = document.getElementById('productSearchResults');
            
            if (products.length === 0) {
                resultsDiv.innerHTML = '<div class="product-search-item text-muted">Ürün bulunamadı</div>';
            } else {
                resultsDiv.innerHTML = products.map(p => `
                    <div class="product-search-item" onclick="selectProduct(${JSON.stringify(p).replace(/"/g, '&quot;')})">
                        <div class="product-info">
                            <span class="product-model">${p.model}</span>
                            <span class="product-ean">${p.ean}</span>
                        </div>
                        <div class="product-details">
                            <span class="color-badge">${p.color}</span>
                            <span class="ms-2">${p.size}</span>
                        </div>
                    </div>
                `).join('');
            }
            
            resultsDiv.classList.add('show');
        })
        .catch(error => {
            console.error('Error searching products:', error);
        });
}

// Select product from search
function selectProduct(product) {
    selectedProduct = product;
    document.getElementById('productSearch').value = `${product.model} - ${product.color} - ${product.size}`;
    document.getElementById('productSearchResults').classList.remove('show');
    document.getElementById('btnAddProduct').disabled = false;
}

// Add product to transfer
function addProductToTransfer() {
    if (!selectedProduct) {
        AlertModal.error('Lütfen bir ürün seçin');
        return;
    }
    
    const quantity = parseInt(document.getElementById('quantityInput').value) || 1;
    
    // Check if product already exists
    const existingIndex = transferDetails.findIndex(d => d.productId === selectedProduct.id);
    if (existingIndex >= 0) {
        transferDetails[existingIndex].quantityReq += quantity;
    } else {
        transferDetails.push({
            productId: selectedProduct.id,
            productModel: selectedProduct.model,
            productColor: selectedProduct.color,
            productSize: selectedProduct.size,
            productEan: selectedProduct.ean,
            quantityReq: quantity
        });
    }
    
    updateDetailsTable();
    
    // Reset
    selectedProduct = null;
    document.getElementById('productSearch').value = '';
    document.getElementById('quantityInput').value = '1';
    document.getElementById('btnAddProduct').disabled = true;
}

// Update details table
function updateDetailsTable() {
    const tbody = document.getElementById('transferDetailsBody');
    const noProductRow = document.getElementById('noProductRow');
    
    if (transferDetails.length === 0) {
        noProductRow.style.display = '';
        return;
    }
    
    noProductRow.style.display = 'none';
    
    // Clear existing rows except the "no product" row
    const existingRows = tbody.querySelectorAll('tr:not(#noProductRow)');
    existingRows.forEach(row => row.remove());
    
    // Add rows
    transferDetails.forEach((detail, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${detail.productModel}</td>
            <td><span class="color-badge">${detail.productColor}</span></td>
            <td>${detail.productSize}</td>
            <td><code>${detail.productEan}</code></td>
            <td>
                <input type="number" class="form-control form-control-sm text-center" 
                       value="${detail.quantityReq}" min="1" style="width: 80px;"
                       onchange="updateDetailQuantity(${index}, this.value)">
            </td>
            <td>
                <button type="button" class="btn btn-danger btn-sm btn-remove-product" 
                        onclick="removeDetailFromTransfer(${index})">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

// Update detail quantity
function updateDetailQuantity(index, value) {
    const qty = parseInt(value) || 1;
    if (qty < 1) return;
    transferDetails[index].quantityReq = qty;
}

// Remove detail from transfer
function removeDetailFromTransfer(index) {
    transferDetails.splice(index, 1);
    updateDetailsTable();
}

// Submit transfer form
function submitTransferForm() {
    const fromShopId = document.getElementById('fromShopId').value;
    const toShopId = document.getElementById('toShopId').value;
    const userId = document.getElementById('userId').value;
    
    if (!fromShopId) {
        AlertModal.error('Lütfen talep eden mağazayı seçin');
        return;
    }
    
    if (!toShopId) {
        AlertModal.error('Lütfen gönderen mağazayı seçin');
        return;
    }
    
    if (fromShopId === toShopId) {
        AlertModal.error('Talep eden ve gönderen mağaza aynı olamaz');
        return;
    }
    
    if (transferDetails.length === 0) {
        AlertModal.error('Lütfen en az bir ürün ekleyin');
        return;
    }
    
    const request = {
        transfer: {
            fromShopId: parseInt(fromShopId),
            toShopId: parseInt(toShopId),
            createBy: parseInt(userId)
        },
        details: transferDetails.map(d => ({
            productId: d.productId,
            quantityReq: d.quantityReq,
            createBy: parseInt(userId)
        }))
    };
    
    fetch('/api/transfer', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => { throw new Error(text); });
        }
        return response.text().then(text => {
            try { return text ? JSON.parse(text) : {}; } catch(e) { return {}; }
        });
    })
    .then(data => {
        const modal = bootstrap.Modal.getInstance(document.getElementById('formModal'));
        if (modal) modal.hide();
        
        setTimeout(() => {
            AlertModal.success('Transfer talebi başarıyla oluşturuldu', () => {
                location.reload();
            });
        }, 300);
    })
    .catch(error => {
        console.error('Error:', error);
        AlertModal.error('Transfer oluşturulurken bir hata oluştu: ' + error.message);
    });
}

// View transfer detail
function viewTransferDetail(transferId) {
    fetch(`/Transfer/DetailForm?id=${transferId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('detailModalContainer');
            container.innerHTML = html;
            
            setupDetailModalListeners();
            
            const modal = new bootstrap.Modal(document.getElementById('detailModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
        showAlert('Detay yüklenirken bir hata oluştu', 'error');
        });
}

// Setup detail modal listeners
function setupDetailModalListeners() {
    const updateButtons = document.querySelectorAll('.btn-update-qty');
    updateButtons.forEach(btn => {
        btn.addEventListener('click', function() {
            const detailId = this.dataset.detailId;
            const userId = this.dataset.userId;
            const input = document.querySelector(`.detail-qty-input[data-detail-id="${detailId}"]`);
            const newQty = parseInt(input.value) || 0;
            
            updateDetailQuantitySent(detailId, newQty, userId);
        });
    });
}

// Update detail quantity sent
function updateDetailQuantitySent(detailId, quantity, userId) {
    fetch(`/api/transfer/detail/${detailId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            quantitySend: quantity,
            updateBy: parseInt(userId)
        })
    })
    .then(response => {
        if (!response.ok) throw new Error('Update failed');
        AlertModal.success('Gönderilen adet güncellendi');
        document.querySelector(`.sent-qty-${detailId}`).textContent = quantity;
    })
    .catch(error => {
        console.error('Error:', error);
        AlertModal.error('Güncelleme sırasında bir hata oluştu');
    });
}

// Update transfer status - modal ile onay
function updateTransferStatus(transferId, status) {
    fetch(`/Transfer/StatusConfirm?id=${transferId}&status=${status}`)
        .then(response => response.text())
        .then(html => {
            // statusModalContainer yoksa oluştur
            let container = document.getElementById('statusModalContainer');
            if (!container) {
                container = document.createElement('div');
                container.id = 'statusModalContainer';
                document.body.appendChild(container);
            }
            container.innerHTML = html;
            
            const modal = new bootstrap.Modal(document.getElementById('statusModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm status update from modal
function confirmStatusUpdate() {
    const transferId = document.getElementById('statusTransferId').value;
    const userId = document.getElementById('statusUserId').value;
    const status = parseInt(document.getElementById('statusNewValue').value);
    
    const statusText = {
        1: 'Gönderildi',
        2: 'Tamamlandı',
        3: 'İptal Edildi'
    };

    // Status 1 (Gönderildi) ise önce detay miktarlarını güncelle
    if (status === 1) {
        const qtyInputs = document.querySelectorAll('.status-qty-input');
        if (qtyInputs.length === 0) {
            updateStatusOnly(transferId, status, userId, statusText);
            return;
        }

        // Miktar validasyonu
        let hasError = false;
        qtyInputs.forEach(input => {
            const val = parseInt(input.value) || 0;
            const max = parseInt(input.max) || 0;
            if (val < 0 || val > max) {
                hasError = true;
                input.classList.add('is-invalid');
            } else {
                input.classList.remove('is-invalid');
            }
        });

        if (hasError) {
            AlertModal.error('Lütfen geçerli miktarlar girin (0 ile istenen arasında)');
            return;
        }

        // Tüm detay miktarlarını sırayla güncelle
        const updatePromises = Array.from(qtyInputs).map(input => {
            const detailId = input.dataset.detailId;
            const qty = parseInt(input.value) || 0;
            return fetch(`/api/transfer/detail/${detailId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    id: parseInt(detailId),
                    quantitySend: qty,
                    updateBy: parseInt(userId)
                })
            });
        });

        Promise.all(updatePromises)
            .then(responses => {
                const allOk = responses.every(r => r.ok);
                if (!allOk) throw new Error('Detail update failed');
                // Miktarlar güncellendi, şimdi durumu güncelle
                return updateStatusOnly(transferId, status, userId, statusText);
            })
            .catch(error => {
                console.error('Error:', error);
                showAlert('Gönderim miktarları güncellenirken bir hata oluştu', 'error');
            });
    } else {
        updateStatusOnly(transferId, status, userId, statusText);
    }
}

// Sadece transfer durumunu güncelle
function updateStatusOnly(transferId, status, userId, statusText) {
    return fetch(`/api/transfer/${transferId}/status`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            status: status,
            updatedBy: parseInt(userId)
        })
    })
    .then(response => {
        if (!response.ok) throw new Error('Update failed');
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('statusModal'));
        if (modal) modal.hide();
        
        setTimeout(() => {
            AlertModal.success(`Transfer durumu "${statusText[status]}" olarak güncellendi`, () => {
                location.reload();
            });
        }, 300);
    })
    .catch(error => {
        console.error('Error:', error);
        AlertModal.error('Durum güncellenirken bir hata oluştu');
    });
}

// Update transfer status from detail modal
function updateTransferStatusFromDetail(transferId, status) {
    const userId = document.querySelector('.btn-update-qty')?.dataset.userId || 0;
    updateTransferStatusWithUserId(transferId, status, userId);
}

// Detay modal'ından gönderildi/tamamlandı yap - önce detay modal kapat, sonra onay modal aç
function statusFromDetail(transferId, status) {
    const detailModal = bootstrap.Modal.getInstance(document.getElementById('detailModal'));
    if (detailModal) detailModal.hide();
    
    setTimeout(() => {
        updateTransferStatus(transferId, status);
    }, 300);
}

// Detay modal'ından iptal et - önce detay modal kapat, sonra iptal modal aç
function cancelFromDetail(transferId) {
    const detailModal = bootstrap.Modal.getInstance(document.getElementById('detailModal'));
    if (detailModal) detailModal.hide();
    
    setTimeout(() => {
        deleteTransfer(transferId);
    }, 300);
}

function updateTransferStatusWithUserId(transferId, status, userId) {
    const statusText = {
        1: 'Gönderildi',
        2: 'Tamamlandı',
        3: 'İptal Edildi'
    };
    
    if (!confirm(`Bu transferi "${statusText[status]}" olarak işaretlemek istediğinizden emin misiniz?`)) {
        return;
    }
    
    fetch(`/api/transfer/${transferId}/status`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            status: status,
            updatedBy: parseInt(userId)
        })
    })
    .then(response => {
        if (!response.ok) throw new Error('Update failed');
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('detailModal'));
        if (modal) modal.hide();
        
        setTimeout(() => {
            AlertModal.success(`Transfer durumu "${statusText[status]}" olarak güncellendi`, () => {
                location.reload();
            });
        }, 300);
    })
    .catch(error => {
        console.error('Error:', error);
        AlertModal.error('Durum güncellenirken bir hata oluştu');
    });
}

// Cancel transfer from detail modal
function cancelTransferFromDetail(transferId) {
    const userId = document.querySelector('.btn-update-qty')?.dataset.userId || 0;
    updateTransferStatusWithUserId(transferId, 3, userId);
}

// Delete transfer (cancel)
function deleteTransfer(transferId) {
    fetch(`/Transfer/DeleteConfirm?id=${transferId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('deleteModalContainer');
            container.innerHTML = html;
            
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete transfer
function confirmDeleteTransfer() {
    const transferId = document.getElementById('deleteTransferId').value;
    const userId = document.getElementById('deleteUserId').value;
    
    fetch(`/api/transfer/${transferId}?updatedBy=${userId}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (!response.ok) throw new Error('Delete failed');
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
        if (modal) modal.hide();
        
        setTimeout(() => {
            AlertModal.success('Transfer başarıyla iptal edildi', () => {
                location.reload();
            });
        }, 300);
    })
    .catch(error => {
        console.error('Error:', error);
        AlertModal.error('İptal işlemi sırasında bir hata oluştu');
    });
}

// Show alert helper - AlertModal kullan
function showAlert(message, type) {
    if (typeof AlertModal !== 'undefined') {
        if (type === 'success') {
            AlertModal.success(message);
        } else if (type === 'error') {
            AlertModal.error(message);
        } else if (type === 'warning') {
            AlertModal.warning(message);
        } else {
            AlertModal.info(message);
        }
        return;
    }
    
    // Fallback alert
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert ${alertClass} alert-dismissible fade show position-fixed`;
    alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    document.body.appendChild(alertDiv);
    
    setTimeout(() => {
        alertDiv.remove();
    }, 3000);
}

// Copy to clipboard function
function copyToClipboard(text, button) {
    // Modern Clipboard API
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(function() {
            // Show success feedback
            showCopyFeedback(button, true);
        }).catch(function(err) {
            console.error('Clipboard API failed:', err);
            // Fallback to execCommand
            fallbackCopyToClipboard(text, button);
        });
    } else {
        // Fallback for older browsers
        fallbackCopyToClipboard(text, button);
    }
}

// Fallback copy method using execCommand (for older browsers)
function fallbackCopyToClipboard(text, button) {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-9999px';
    document.body.appendChild(textArea);
    textArea.select();
    
    try {
        const successful = document.execCommand('copy');
        showCopyFeedback(button, successful);
    } catch (err) {
        console.error('Fallback copy failed:', err);
        showCopyFeedback(button, false);
    }
    
    document.body.removeChild(textArea);
}

// Show visual feedback for copy action
function showCopyFeedback(button, success) {
    const icon = button.querySelector('i');
    const originalClass = icon.className;
    
    if (success) {
        // Change icon to check mark
        icon.className = 'fas fa-check text-success';
        button.classList.add('copy-success');
        
        // Show toast notification
        showToast('Transfer kodu kopyalandı!', 'success');
        
        // Reset after 2 seconds
        setTimeout(() => {
            icon.className = originalClass;
            button.classList.remove('copy-success');
        }, 2000);
    } else {
        // Show error feedback
        icon.className = 'fas fa-times text-danger';
        showToast('Kopyalama başarısız!', 'error');
        
        // Reset after 2 seconds
        setTimeout(() => {
            icon.className = originalClass;
        }, 2000);
    }
}

// Copy transfer code to clipboard
function copyTransferCode(text, iconElement) {
    // Modern Clipboard API
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(function() {
            showCopySuccess(iconElement);
        }).catch(function(err) {
            console.error('Clipboard API failed:', err);
            fallbackCopy(text, iconElement);
        });
    } else {
        fallbackCopy(text, iconElement);
    }
}

// Fallback copy method for older browsers
function fallbackCopy(text, iconElement) {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-9999px';
    document.body.appendChild(textArea);
    textArea.select();
    
    try {
        const successful = document.execCommand('copy');
        if (successful) {
            showCopySuccess(iconElement);
        } else {
            // Show error toast only, keep copy icon
            showToast('Kopyalama başarısız!', 'error');
        }
    } catch (err) {
        console.error('Fallback copy failed:', err);
        // Show error toast only, keep copy icon
        showToast('Kopyalama başarısız!', 'error');
    }
    
    document.body.removeChild(textArea);
}

// Show success feedback
function showCopySuccess(iconElement) {
    // Just show toast, keep the copy icon unchanged
    showToast('Transfer kodu kopyalandı!', 'success');
}
