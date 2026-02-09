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

// Setup filters
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

// Apply filters
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
            select.innerHTML = '<option value="">-- Mağaza Seçiniz --</option>';
            shops.forEach(shop => {
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
        showAlert('Lütfen bir ürün seçin', 'error');
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
        showAlert('Lütfen talep eden mağazayı seçin', 'error');
        return;
    }
    
    if (!toShopId) {
        showAlert('Lütfen gönderen mağazayı seçin', 'error');
        return;
    }
    
    if (fromShopId === toShopId) {
        showAlert('Talep eden ve gönderen mağaza aynı olamaz', 'error');
        return;
    }
    
    if (transferDetails.length === 0) {
        showAlert('Lütfen en az bir ürün ekleyin', 'error');
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
        return response.json();
    })
    .then(data => {
        showAlert('Transfer talebi başarıyla oluşturuldu', 'success');
        const modal = bootstrap.Modal.getInstance(document.getElementById('formModal'));
        modal.hide();
        setTimeout(() => location.reload(), 1000);
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Transfer oluşturulurken bir hata oluştu: ' + error.message, 'error');
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
        showAlert('Gönderilen adet güncellendi', 'success');
        document.querySelector(`.sent-qty-${detailId}`).textContent = quantity;
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Güncelleme sırasında bir hata oluştu', 'error');
    });
}

// Update transfer status
function updateTransferStatus(transferId, status) {
    const userId = document.querySelector('[data-user-id]')?.dataset.userId || 
                   document.getElementById('userId')?.value || 0;
    
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
        showAlert(`Transfer durumu "${statusText[status]}" olarak güncellendi`, 'success');
        setTimeout(() => location.reload(), 1000);
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Durum güncellenirken bir hata oluştu', 'error');
    });
}

// Update transfer status from detail modal
function updateTransferStatusFromDetail(transferId, status) {
    const userId = document.querySelector('.btn-update-qty')?.dataset.userId || 0;
    updateTransferStatusWithUserId(transferId, status, userId);
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
        showAlert(`Transfer durumu "${statusText[status]}" olarak güncellendi`, 'success');
        
        // Close modal and reload
        const modal = bootstrap.Modal.getInstance(document.getElementById('detailModal'));
        if (modal) modal.hide();
        setTimeout(() => location.reload(), 1000);
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Durum güncellenirken bir hata oluştu', 'error');
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
        showAlert('Transfer iptal edildi', 'success');
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
        modal.hide();
        setTimeout(() => location.reload(), 1000);
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('İptal işlemi sırasında bir hata oluştu', 'error');
    });
}

// Show alert helper
function showAlert(message, type) {
    // Check if site.js has showAlert function
    if (typeof window.showAlert === 'function') {
        window.showAlert(message, type);
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
