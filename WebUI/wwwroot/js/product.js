// Product Management JavaScript - Option Bazlı Sistem

document.addEventListener('DOMContentLoaded', function () {
    // Add Product (Option) Button
    const btnAddProduct = document.getElementById('btnAddProduct');
    if (btnAddProduct) {
        btnAddProduct.addEventListener('click', function () {
            loadCreateForm();
        });
    }
});

// ========================================
// Option Bazlı Fonksiyonlar
// ========================================

// Option Detay Modalı Aç
function openOptionDetail(model, color) {
    fetch(`/Product/OptionDetailModal?model=${encodeURIComponent(model)}&color=${encodeURIComponent(color)}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('optionDetailModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('optionDetailModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Detay yüklenirken bir hata oluştu.', 'error');
        });
}

// Option Detay Yenile
function refreshOptionDetail(model, color) {
    // Önce mevcut modalı kapat
    const existingModal = document.getElementById('optionDetailModal');
    if (existingModal) {
        const modalInstance = bootstrap.Modal.getInstance(existingModal);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
    
    // Sayfayı yenile veya modalı tekrar aç
    setTimeout(() => {
        openOptionDetail(model, color);
    }, 300);
}

// ========================================
// Beden (Size) Fonksiyonları
// ========================================

// Yeni Beden Ekleme Formu Aç
function openSizeCreateForm(model, color) {
    fetch(`/Product/SizeCreateForm?model=${encodeURIComponent(model)}&color=${encodeURIComponent(color)}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('sizeFormModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('sizeFormModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Form yüklenirken bir hata oluştu.', 'error');
        });
}

// Beden Düzenle
function editSize(id) {
    fetch(`/Product/SizeEditForm?id=${id}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('sizeFormModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('sizeFormModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Form yüklenirken bir hata oluştu.', 'error');
        });
}

// Beden Sil Onay
function deleteSize(id) {
    fetch(`/Product/SizeDeleteConfirm?id=${id}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('sizeDeleteModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('sizeDeleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Modal yüklenirken bir hata oluştu.', 'error');
        });
}

// ========================================
// ProductShelf (Raf Ataması) Fonksiyonları
// ========================================

// Ürün Raf Modalı Aç (Beden bazlı)
function openProductShelfModal(productId) {
    fetch(`/Product/ProductShelfCreateForm?productId=${productId}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('productShelfFormModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('productShelfFormModal'));
            modal.show();
            initializeProductShelfFormEvents();
            // Mevcut raf atamalarını yükle
            loadProductShelves(productId);
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Form yüklenirken bir hata oluştu.', 'error');
        });
}

// ========================================
// Yeni Option Ekleme (İlk beden ile birlikte)
// ========================================

// Load Create Form - Yeni Option (Model + Color + İlk Beden)
function loadCreateForm() {
    fetch('/Product/CreateForm')
        .then(response => response.text())
        .then(html => {
            document.getElementById('formModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('formModal'));
            modal.show();
            initializeProductFormEvents();
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Form yüklenirken bir hata oluştu.', 'error');
        });
}

// Initialize Product Form Events
function initializeProductFormEvents() {
    const form = document.getElementById('productForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            submitProductForm();
        });
    }
}

// Submit Product Form
function submitProductForm() {
    const form = document.getElementById('productForm');
    const formData = new FormData(form);
    const id = document.getElementById('productId').value;

    // EAN validation - must be exactly 13 digits
    const ean = document.getElementById('productEan').value;
    if (!/^[0-9]{13}$/.test(ean)) {
        showToast('EAN kodu tam 13 haneli rakam olmalıdır.', 'warning');
        return;
    }

    // Handle IsActive checkbox
    const isActiveCheckbox = document.getElementById('productIsActive');
    formData.set('IsActive', isActiveCheckbox.checked);

    const url = id && id !== '0'
        ? `/Product/Update?id=${id}`
        : '/Product/CreateJson';

    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const modal = bootstrap.Modal.getInstance(document.getElementById('formModal'));
                modal.hide();
                showToast(data.message, 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(data.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('İşlem sırasında bir hata oluştu.', 'error');
        });
}

// Load cities for detail modal
function loadCitiesForDetail(regionId) {
    const citySelect = document.getElementById('detailCity');
    const townSelect = document.getElementById('detailTown');
    const shopSelect = document.getElementById('detailShop');
    const warehouseSelect = document.getElementById('detailWarehouse');
    const shelfSelect = document.getElementById('detailShelf');

    // Reset dependent dropdowns
    citySelect.innerHTML = '<option value="">-- Seçiniz --</option>';
    townSelect.innerHTML = '<option value="">-- Önce Şehir --</option>';
    shopSelect.innerHTML = '<option value="">-- Önce İlçe --</option>';
    warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza --</option>';
    shelfSelect.innerHTML = '<option value="">-- Önce Depo --</option>';

    citySelect.disabled = true;
    townSelect.disabled = true;
    shopSelect.disabled = true;
    warehouseSelect.disabled = true;
    shelfSelect.disabled = true;

    if (!regionId) return;

    fetch(`/Product/GetCitiesByRegion?regionId=${regionId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(city => {
                const option = document.createElement('option');
                option.value = city.id;
                option.textContent = city.name;
                citySelect.appendChild(option);
            });
            citySelect.disabled = false;
        })
        .catch(error => console.error('Error loading cities:', error));
}

// Load towns for detail modal
function loadTownsForDetail(cityId) {
    const townSelect = document.getElementById('detailTown');
    const shopSelect = document.getElementById('detailShop');
    const warehouseSelect = document.getElementById('detailWarehouse');
    const shelfSelect = document.getElementById('detailShelf');

    townSelect.innerHTML = '<option value="">-- Seçiniz --</option>';
    shopSelect.innerHTML = '<option value="">-- Önce İlçe --</option>';
    warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza --</option>';
    shelfSelect.innerHTML = '<option value="">-- Önce Depo --</option>';

    townSelect.disabled = true;
    shopSelect.disabled = true;
    warehouseSelect.disabled = true;
    shelfSelect.disabled = true;

    if (!cityId) return;

    fetch(`/Product/GetTownsByCity?cityId=${cityId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(town => {
                const option = document.createElement('option');
                option.value = town.id;
                option.textContent = town.name;
                townSelect.appendChild(option);
            });
            townSelect.disabled = false;
        })
        .catch(error => console.error('Error loading towns:', error));
}

// Load shops for detail modal
function loadShopsForDetail(townId) {
    const shopSelect = document.getElementById('detailShop');
    const warehouseSelect = document.getElementById('detailWarehouse');
    const shelfSelect = document.getElementById('detailShelf');

    shopSelect.innerHTML = '<option value="">-- Seçiniz --</option>';
    warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza --</option>';
    shelfSelect.innerHTML = '<option value="">-- Önce Depo --</option>';

    shopSelect.disabled = true;
    warehouseSelect.disabled = true;
    shelfSelect.disabled = true;

    if (!townId) return;

    fetch(`/Product/GetShopsByTown?townId=${townId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(shop => {
                const option = document.createElement('option');
                option.value = shop.id;
                option.textContent = shop.name;
                shopSelect.appendChild(option);
            });
            shopSelect.disabled = false;
        })
        .catch(error => console.error('Error loading shops:', error));
}

// Load warehouses for detail modal
function loadWarehousesForDetail(shopId) {
    const warehouseSelect = document.getElementById('detailWarehouse');
    const shelfSelect = document.getElementById('detailShelf');

    warehouseSelect.innerHTML = '<option value="">-- Seçiniz --</option>';
    shelfSelect.innerHTML = '<option value="">-- Önce Depo --</option>';

    warehouseSelect.disabled = true;
    shelfSelect.disabled = true;

    if (!shopId) return;

    fetch(`/Product/GetWarehousesByShop?shopId=${shopId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(warehouse => {
                const option = document.createElement('option');
                option.value = warehouse.id;
                option.textContent = warehouse.name;
                warehouseSelect.appendChild(option);
            });
            warehouseSelect.disabled = false;
        })
        .catch(error => console.error('Error loading warehouses:', error));
}

// Load shelves for detail modal
function loadShelvesForDetail(warehouseId) {
    const shelfSelect = document.getElementById('detailShelf');

    shelfSelect.innerHTML = '<option value="">-- Seçiniz --</option>';
    shelfSelect.disabled = true;

    if (!warehouseId) return;

    fetch(`/Product/GetShelvesByWarehouse?warehouseId=${warehouseId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(shelf => {
                const option = document.createElement('option');
                option.value = shelf.id;
                option.textContent = shelf.code;
                shelfSelect.appendChild(option);
            });
            shelfSelect.disabled = false;
        })
        .catch(error => console.error('Error loading shelves:', error));
}

// Load Product Shelves List
function loadProductShelves(productId) {
    const tbody = document.getElementById('productShelvesBody');

    fetch(`/Product/GetProductShelves?productId=${productId}`)
        .then(response => response.json())
        .then(data => {
            if (data.length === 0) {
                tbody.innerHTML = `
                    <tr>
                        <td colspan="8" class="text-center text-muted">
                            <i class="fas fa-inbox me-2"></i>Bu ürün için henüz stok kaydı bulunmuyor.
                        </td>
                    </tr>
                `;
                return;
            }

            tbody.innerHTML = '';
            data.forEach(item => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${item.regionName || '-'}</td>
                    <td>${item.cityName || '-'}</td>
                    <td>${item.townName || '-'}</td>
                    <td>${item.shopName || '-'}</td>
                    <td>${item.warehouseName || '-'}</td>
                    <td><span class="badge bg-dark">${item.shelfCode}</span></td>
                    <td><span class="badge bg-primary">${item.quantity}</span></td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary me-1" onclick="editProductShelf(${item.id})">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteProductShelf(${item.id})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;
                tbody.appendChild(row);
            });
        })
        .catch(error => {
            console.error('Error:', error);
            tbody.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center text-danger">
                        <i class="fas fa-exclamation-triangle me-2"></i>Veriler yüklenirken bir hata oluştu.
                    </td>
                </tr>
            `;
        });
}

// Add ProductShelf from Detail Modal
function addProductShelfFromDetail() {
    const productId = document.getElementById('detailProductId').value;
    const shelfId = document.getElementById('detailShelf').value;
    const quantity = document.getElementById('detailQuantity').value;

    if (!shelfId) {
        showAlert('warning', 'Lütfen bir raf seçiniz.');
        return;
    }

    if (!quantity || quantity < 1) {
        showAlert('warning', 'Lütfen geçerli bir adet giriniz.');
        return;
    }

    const formData = new FormData();
    formData.append('ProductId', productId);
    formData.append('ShelfId', shelfId);
    formData.append('Quantity', quantity);

    fetch('/Product/CreateProductShelf', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showAlert('success', data.message);
                loadProductShelves(productId);
                // Reset form
                document.getElementById('detailRegion').value = '';
                document.getElementById('detailCity').innerHTML = '<option value="">-- Önce Bölge --</option>';
                document.getElementById('detailCity').disabled = true;
                document.getElementById('detailTown').innerHTML = '<option value="">-- Önce Şehir --</option>';
                document.getElementById('detailTown').disabled = true;
                document.getElementById('detailShop').innerHTML = '<option value="">-- Önce İlçe --</option>';
                document.getElementById('detailShop').disabled = true;
                document.getElementById('detailWarehouse').innerHTML = '<option value="">-- Önce Mağaza --</option>';
                document.getElementById('detailWarehouse').disabled = true;
                document.getElementById('detailShelf').innerHTML = '<option value="">-- Önce Depo --</option>';
                document.getElementById('detailShelf').disabled = true;
                document.getElementById('detailQuantity').value = '1';
            } else {
                showAlert('danger', data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('danger', 'Stok eklenirken bir hata oluştu.');
        });
}

// Edit ProductShelf
function editProductShelf(id) {
    fetch(`/Product/ProductShelfEditForm?id=${id}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('productShelfFormModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('productShelfFormModal'));
            modal.show();
            initializeProductShelfFormEvents();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('danger', 'Form yüklenirken bir hata oluştu.');
        });
}

// Delete ProductShelf
function deleteProductShelf(id) {
    fetch(`/Product/ProductShelfDeleteConfirm?id=${id}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('productShelfDeleteModalContainer').innerHTML = html;
            const modal = new bootstrap.Modal(document.getElementById('productShelfDeleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('danger', 'Modal yüklenirken bir hata oluştu.');
        });
}

// Initialize ProductShelf Form Events
function initializeProductShelfFormEvents() {
    const form = document.getElementById('productShelfForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            submitProductShelfForm();
        });
    }

    // Cascade dropdowns for ProductShelf form
    const regionSelect = document.getElementById('psRegion');
    if (regionSelect) {
        regionSelect.addEventListener('change', function () {
            loadCitiesForPS(this.value);
        });
    }

    const citySelect = document.getElementById('psCity');
    if (citySelect) {
        citySelect.addEventListener('change', function () {
            loadTownsForPS(this.value);
        });
    }

    const townSelect = document.getElementById('psTown');
    if (townSelect) {
        townSelect.addEventListener('change', function () {
            loadShopsForPS(this.value);
        });
    }

    const shopSelect = document.getElementById('psShop');
    if (shopSelect) {
        shopSelect.addEventListener('change', function () {
            loadWarehousesForPS(this.value);
        });
    }

    const warehouseSelect = document.getElementById('psWarehouse');
    if (warehouseSelect) {
        warehouseSelect.addEventListener('change', function () {
            loadShelvesForPS(this.value);
        });
    }
}

// ProductShelf Form Cascade Functions
function loadCitiesForPS(regionId) {
    const citySelect = document.getElementById('psCity');
    citySelect.innerHTML = '<option value="">-- Şehir Seçiniz --</option>';
    document.getElementById('psTown').innerHTML = '<option value="">-- Önce Şehir Seçiniz --</option>';
    document.getElementById('psShop').innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
    document.getElementById('psWarehouse').innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
    document.getElementById('psShelf').innerHTML = '<option value="">-- Önce Depo Seçiniz --</option>';

    if (!regionId) return;

    fetch(`/Product/GetCitiesByRegion?regionId=${regionId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(city => {
                const option = document.createElement('option');
                option.value = city.id;
                option.textContent = city.name;
                citySelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error loading cities:', error));
}

function loadTownsForPS(cityId) {
    const townSelect = document.getElementById('psTown');
    townSelect.innerHTML = '<option value="">-- İlçe Seçiniz --</option>';
    document.getElementById('psShop').innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
    document.getElementById('psWarehouse').innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
    document.getElementById('psShelf').innerHTML = '<option value="">-- Önce Depo Seçiniz --</option>';

    if (!cityId) return;

    fetch(`/Product/GetTownsByCity?cityId=${cityId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(town => {
                const option = document.createElement('option');
                option.value = town.id;
                option.textContent = town.name;
                townSelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error loading towns:', error));
}

function loadShopsForPS(townId) {
    const shopSelect = document.getElementById('psShop');
    shopSelect.innerHTML = '<option value="">-- Mağaza Seçiniz --</option>';
    document.getElementById('psWarehouse').innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
    document.getElementById('psShelf').innerHTML = '<option value="">-- Önce Depo Seçiniz --</option>';

    if (!townId) return;

    fetch(`/Product/GetShopsByTown?townId=${townId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(shop => {
                const option = document.createElement('option');
                option.value = shop.id;
                option.textContent = shop.name;
                shopSelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error loading shops:', error));
}

function loadWarehousesForPS(shopId) {
    const warehouseSelect = document.getElementById('psWarehouse');
    warehouseSelect.innerHTML = '<option value="">-- Depo Seçiniz --</option>';
    document.getElementById('psShelf').innerHTML = '<option value="">-- Önce Depo Seçiniz --</option>';

    if (!shopId) return;

    fetch(`/Product/GetWarehousesByShop?shopId=${shopId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(warehouse => {
                const option = document.createElement('option');
                option.value = warehouse.id;
                option.textContent = warehouse.name;
                warehouseSelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error loading warehouses:', error));
}

function loadShelvesForPS(warehouseId) {
    const shelfSelect = document.getElementById('psShelf');
    shelfSelect.innerHTML = '<option value="">-- Raf Seçiniz --</option>';

    if (!warehouseId) return;

    fetch(`/Product/GetShelvesByWarehouse?warehouseId=${warehouseId}`)
        .then(response => response.json())
        .then(data => {
            data.forEach(shelf => {
                const option = document.createElement('option');
                option.value = shelf.id;
                option.textContent = shelf.code;
                shelfSelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error loading shelves:', error));
}

// Submit ProductShelf Form
function submitProductShelfForm() {
    const form = document.getElementById('productShelfForm');
    const formData = new FormData(form);
    const id = document.getElementById('productShelfId').value;
    const productId = document.getElementById('productShelfProductId').value;

    const url = id && id !== '0'
        ? `/Product/UpdateProductShelf?id=${id}`
        : '/Product/CreateProductShelf';

    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const modal = bootstrap.Modal.getInstance(document.getElementById('productShelfFormModal'));
                modal.hide();
                showAlert('success', data.message);
                loadProductShelves(productId);
            } else {
                showAlert('danger', data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('danger', 'İşlem sırasında bir hata oluştu.');
        });
}

// Confirm Delete ProductShelf
function confirmDeleteProductShelf() {
    const id = document.getElementById('deleteProductShelfId').value;
    const productId = document.getElementById('detailProductId').value;

    fetch(`/Product/DeleteProductShelf?id=${id}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const modal = bootstrap.Modal.getInstance(document.getElementById('productShelfDeleteModal'));
                modal.hide();
                showAlert('success', data.message);
                loadProductShelves(productId);
            } else {
                showAlert('danger', data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('danger', 'Silme işlemi sırasında bir hata oluştu.');
        });
}

// Show Alert
function showAlert(type, message) {
    // Remove existing alerts
    const existingAlerts = document.querySelectorAll('.custom-alert');
    existingAlerts.forEach(alert => alert.remove());

    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show custom-alert`;
    alertDiv.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    document.body.appendChild(alertDiv);

    // Auto remove after 3 seconds
    setTimeout(() => {
        alertDiv.remove();
    }, 3000);
}

// Show Toast - Daha modern bildirim
function showToast(message, type = 'info') {
    const alertType = type === 'success' ? 'success' : 
                      type === 'error' ? 'danger' : 
                      type === 'warning' ? 'warning' : 'info';
    showAlert(alertType, message);
}

// ========================================
// Quick Transfer (Hızlı Transfer Talebi)
// ========================================

// Transfer Talep Et - Beden bazlı (Global fonksiyon)
let currentTransferData = null;

async function handleQuickTransferForSize(productId) {
    const userId = document.getElementById('currentUserId')?.value;
    const userShopId = document.getElementById('currentUserShopId')?.value;
    
    if (!userId || !userShopId) {
        alert('Kullanıcı bilgileri bulunamadı.');
        return;
    }

    try {
        // 1. En fazla stok bulunan mağazayı bul
        const response = await fetch(`/Product/GetShopWithMaxStock?productId=${productId}&excludeShopId=${userShopId}`);
        const result = await response.json();
        
        if (!result.success || !result.data) {
            alert(result.message || 'Bu ürün için başka mağazalarda stok bulunamadı.');
            return;
        }

        const shopStock = result.data;
        
        // Transfer bilgilerini sakla
        currentTransferData = {
            productId: productId,
            userId: userId,
            userShopId: userShopId,
            shopStock: shopStock
        };

        // Modal'ı doldur ve göster
        document.getElementById('transferShopName').textContent = shopStock.shopName;
        document.getElementById('transferAvailableStock').textContent = shopStock.totalStock;
        document.getElementById('transferMaxQuantity').textContent = shopStock.totalStock;
        document.getElementById('transferQuantity').value = 1;
        document.getElementById('transferQuantity').max = shopStock.totalStock;

        const modal = new bootstrap.Modal(document.getElementById('quickTransferModal'));
        modal.show();

    } catch (error) {
        console.error('Error:', error);
        alert('❌ Bir hata oluştu: ' + error.message);
    }
}

// Confirm Transfer butonu - Event Delegation kullanarak
document.addEventListener('click', async function(e) {
    if (e.target && e.target.id === 'btnConfirmTransfer') {
        if (!currentTransferData) return;

        const quantity = parseInt(document.getElementById('transferQuantity').value);
        
        if (!quantity || quantity <= 0) {
            alert('Lütfen geçerli bir miktar girin.');
            return;
        }

        if (quantity > currentTransferData.shopStock.totalStock) {
            alert(`Maksimum ${currentTransferData.shopStock.totalStock} adet talep edebilirsiniz.`);
            return;
        }

        // Buton disable
        const btn = e.target;
        const originalHtml = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Oluşturuluyor...';

        try {
            // Transfer talebi oluştur
            const transferResponse = await fetch('/Product/CreateQuickTransfer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    fromShopId: parseInt(currentTransferData.userShopId),
                    toShopId: currentTransferData.shopStock.shopId,
                    productId: parseInt(currentTransferData.productId),
                    quantity: quantity,
                    userId: parseInt(currentTransferData.userId)
                })
            });

            const transferResult = await transferResponse.json();

            // Modal'ı kapat
            const modal = bootstrap.Modal.getInstance(document.getElementById('quickTransferModal'));
            if (modal) modal.hide();

            if (transferResult.success && transferResult.data) {
                // Başarılı modal göster
                showSuccessModal(
                    transferResult.data.transferId,
                    currentTransferData.shopStock.shopName,
                    quantity
                );
            } else {
                showAlert('danger', '❌ ' + (transferResult.message || 'Transfer talebi oluşturulamadı.'));
            }

        } catch (error) {
            console.error('Error:', error);
            showAlert('danger', '❌ Bir hata oluştu: ' + error.message);
        } finally {
            btn.disabled = false;
            btn.innerHTML = originalHtml;
            currentTransferData = null;
        }
    }
});

// Başarılı Transfer Modal Göster
function showSuccessModal(transferId, shopName, quantity) {
    const modalHtml = `
        <div class="modal fade" id="transferSuccessModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-0 shadow-lg">
                    <div class="modal-body text-center p-5">
                        <div class="mb-4">
                            <i class="fas fa-check-circle text-success" style="font-size: 80px;"></i>
                        </div>
                        <h3 class="text-success mb-3">Transfer Talebi Oluşturuldu!</h3>
                        <div class="alert alert-light border mb-4">
                            <div class="row text-start">
                                <div class="col-5 fw-bold">Transfer No:</div>
                                <div class="col-7">#${transferId}</div>
                            </div>
                            <hr class="my-2">
                            <div class="row text-start">
                                <div class="col-5 fw-bold">Gönderici Mağaza:</div>
                                <div class="col-7">${shopName}</div>
                            </div>
                            <hr class="my-2">
                            <div class="row text-start">
                                <div class="col-5 fw-bold">Miktar:</div>
                                <div class="col-7">${quantity} adet</div>
                            </div>
                        </div>
                        <p class="text-muted mb-4">
                            <i class="fas fa-info-circle me-1"></i>
                            Transfer talebi başarıyla oluşturuldu. Mağaza onayını bekleyebilirsiniz.
                        </p>
                        <button type="button" class="btn btn-success btn-lg px-5" data-bs-dismiss="modal">
                            <i class="fas fa-check me-2"></i>Tamam
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Eski modalı temizle
    const oldModal = document.getElementById('transferSuccessModal');
    if (oldModal) oldModal.remove();
    
    // Yeni modalı ekle
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Modal'ı göster
    const modal = new bootstrap.Modal(document.getElementById('transferSuccessModal'));
    modal.show();
    
    // Modal kapandığında temizle
    document.getElementById('transferSuccessModal').addEventListener('hidden.bs.modal', function() {
        this.remove();
    });
}

// Quick Transfer butonu için event listener (ESKİ - artık gerek yok)
document.addEventListener('DOMContentLoaded', function() {
    // Detail modal açıldığında btnQuickTransfer'ı dinle
    const observer = new MutationObserver(function(mutations) {
        const btnQuickTransfer = document.getElementById('btnQuickTransfer');
        const btnQuickTransferFooter = document.getElementById('btnQuickTransferFooter');
        
        if (btnQuickTransfer && !btnQuickTransfer.dataset.listenerAdded) {
            btnQuickTransfer.dataset.listenerAdded = 'true';
            btnQuickTransfer.addEventListener('click', handleQuickTransfer);
        }
        
        if (btnQuickTransferFooter && !btnQuickTransferFooter.dataset.listenerAdded) {
            btnQuickTransferFooter.dataset.listenerAdded = 'true';
            btnQuickTransferFooter.addEventListener('click', handleQuickTransfer);
        }
    });
    
    observer.observe(document.body, { childList: true, subtree: true });
});

async function handleQuickTransfer() {
    const productId = document.getElementById('detailProductId')?.value;
    const userId = document.getElementById('currentUserId')?.value;
    const userShopId = document.getElementById('currentUserShopId')?.value;
    
    if (!productId || !userId || !userShopId) {
        showAlert('danger', 'Kullanıcı bilgileri bulunamadı.');
        return;
    }

    // Buton disable
    const btn = document.getElementById('btnQuickTransfer');
    const originalHtml = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>İşleniyor...';

    try {
        // 1. En fazla stok bulunan mağazayı bul
        const response = await fetch(`https://localhost:7234/api/product/${productId}/max-stock-shop?excludeShopId=${userShopId}`);
        
        if (!response.ok) {
            const error = await response.json();
            showAlert('warning', error.message || 'Bu ürün için başka mağazalarda stok bulunamadı.');
            btn.disabled = false;
            btn.innerHTML = originalHtml;
            return;
        }

        const shopStock = await response.json();
        
        // 2. Miktar sor
        const quantity = prompt(`${shopStock.shopName} mağazasında ${shopStock.totalStock} adet stok var.\n\nKaç adet talep etmek istiyorsunuz?`, '1');
        
        if (!quantity || parseInt(quantity) <= 0) {
            showAlert('info', 'Transfer talebi iptal edildi.');
            btn.disabled = false;
            btn.innerHTML = originalHtml;
            return;
        }

        // 3. Transfer talebi oluştur
        const transferResponse = await fetch('https://localhost:7234/api/transfer/quick', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                fromShopId: parseInt(userShopId),      // Talep eden (ben)
                toShopId: shopStock.shopId,            // Gönderen (en fazla stok olan)
                productId: parseInt(productId),
                quantity: parseInt(quantity),
                userId: parseInt(userId)
            })
        });

        const result = await transferResponse.json();

        if (transferResponse.ok) {
            showAlert('success', `Transfer talebi oluşturuldu! (Transfer #${result.transferId})`);
        } else {
            showAlert('danger', result.message || 'Transfer talebi oluşturulamadı.');
        }

    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Bir hata oluştu: ' + error.message);
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalHtml;
    }
}

