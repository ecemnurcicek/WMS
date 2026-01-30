// Shelf Management JavaScript

let currentShelfId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeShelfManagement();
});

function initializeShelfManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new shelf button
    const btnAddShelf = document.getElementById('btnAddShelf');
    if (btnAddShelf) {
        btnAddShelf.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Attach Region change listener for cascading dropdown
function attachRegionChangeListener() {
    const regionSelect = document.getElementById('shelfRegion');
    if (regionSelect) {
        regionSelect.addEventListener('change', function () {
            const regionId = this.value;
            const citySelect = document.getElementById('shelfCity');
            const townSelect = document.getElementById('shelfTown');
            const shopSelect = document.getElementById('shelfShop');
            const warehouseSelect = document.getElementById('shelfWarehouse');
            
            // Reset all dependent dropdowns
            citySelect.innerHTML = '<option value="">-- Şehir Yükleniyor... --</option>';
            townSelect.innerHTML = '<option value="">-- Önce Şehir Seçiniz --</option>';
            shopSelect.innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
            warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
            
            if (regionId) {
                fetch(`/Shelf/GetCitiesByRegion?regionId=${regionId}`)
                    .then(response => response.json())
                    .then(data => {
                        citySelect.innerHTML = '<option value="">-- Şehir Seçiniz --</option>';
                        data.forEach(city => {
                            const option = document.createElement('option');
                            option.value = city.id;
                            option.textContent = city.name;
                            citySelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error loading cities:', error);
                        citySelect.innerHTML = '<option value="">-- Şehir Yüklenemedi --</option>';
                    });
            } else {
                citySelect.innerHTML = '<option value="">-- Önce Bölge Seçiniz --</option>';
            }
        });
    }
}

// Attach City change listener for cascading dropdown
function attachCityChangeListener() {
    const citySelect = document.getElementById('shelfCity');
    if (citySelect) {
        citySelect.addEventListener('change', function () {
            const cityId = this.value;
            const townSelect = document.getElementById('shelfTown');
            const shopSelect = document.getElementById('shelfShop');
            const warehouseSelect = document.getElementById('shelfWarehouse');
            
            // Reset dependent dropdowns
            townSelect.innerHTML = '<option value="">-- İlçe Yükleniyor... --</option>';
            shopSelect.innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
            warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
            
            if (cityId) {
                fetch(`/Shelf/GetTownsByCity?cityId=${cityId}`)
                    .then(response => response.json())
                    .then(data => {
                        townSelect.innerHTML = '<option value="">-- İlçe Seçiniz --</option>';
                        data.forEach(town => {
                            const option = document.createElement('option');
                            option.value = town.id;
                            option.textContent = town.name;
                            townSelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error loading towns:', error);
                        townSelect.innerHTML = '<option value="">-- İlçe Yüklenemedi --</option>';
                    });
            } else {
                townSelect.innerHTML = '<option value="">-- Önce Şehir Seçiniz --</option>';
            }
        });
    }
}

// Attach Town change listener for cascading dropdown
function attachTownChangeListener() {
    const townSelect = document.getElementById('shelfTown');
    if (townSelect) {
        townSelect.addEventListener('change', function () {
            const townId = this.value;
            const shopSelect = document.getElementById('shelfShop');
            const warehouseSelect = document.getElementById('shelfWarehouse');
            
            // Reset dependent dropdowns
            shopSelect.innerHTML = '<option value="">-- Mağaza Yükleniyor... --</option>';
            warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
            
            if (townId) {
                fetch(`/Shelf/GetShopsByTown?townId=${townId}`)
                    .then(response => response.json())
                    .then(data => {
                        shopSelect.innerHTML = '<option value="">-- Mağaza Seçiniz --</option>';
                        data.forEach(shop => {
                            const option = document.createElement('option');
                            option.value = shop.id;
                            option.textContent = shop.name;
                            shopSelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error loading shops:', error);
                        shopSelect.innerHTML = '<option value="">-- Mağaza Yüklenemedi --</option>';
                    });
            } else {
                shopSelect.innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
            }
        });
    }
}

// Attach Shop change listener for cascading dropdown
function attachShopChangeListener() {
    const shopSelect = document.getElementById('shelfShop');
    if (shopSelect) {
        shopSelect.addEventListener('change', function () {
            const shopId = this.value;
            const warehouseSelect = document.getElementById('shelfWarehouse');
            
            // Reset warehouse dropdown
            warehouseSelect.innerHTML = '<option value="">-- Depo Yükleniyor... --</option>';
            
            if (shopId) {
                fetch(`/Shelf/GetWarehousesByShop?shopId=${shopId}`)
                    .then(response => response.json())
                    .then(data => {
                        warehouseSelect.innerHTML = '<option value="">-- Depo Seçiniz --</option>';
                        data.forEach(warehouse => {
                            const option = document.createElement('option');
                            option.value = warehouse.id;
                            option.textContent = warehouse.name;
                            warehouseSelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error loading warehouses:', error);
                        warehouseSelect.innerHTML = '<option value="">-- Depo Yüklenemedi --</option>';
                    });
            } else {
                warehouseSelect.innerHTML = '<option value="">-- Önce Mağaza Seçiniz --</option>';
            }
        });
    }
}

// Open Create Modal
function openCreateModal() {
    fetch('/Shelf/CreateForm')
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('formModalContainer');
            if (container) {
                container.innerHTML = html;
            } else {
                const div = document.createElement('div');
                div.id = 'formModalContainer';
                div.innerHTML = html;
                document.body.appendChild(div);
            }
            
            // Attach cascading dropdown listeners
            attachRegionChangeListener();
            attachCityChangeListener();
            attachTownChangeListener();
            attachShopChangeListener();
            
            // Re-attach form submission listener
            const form = document.getElementById('shelfForm');
            if (form) {
                form.removeEventListener('submit', submitShelfForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitShelfForm();
                });
            }
            
            const modal = new bootstrap.Modal(document.getElementById('formModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Edit shelf
function editShelf(shelfId) {
    fetch(`/Shelf/EditForm?id=${shelfId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('formModalContainer');
            if (container) {
                container.innerHTML = html;
            } else {
                const div = document.createElement('div');
                div.id = 'formModalContainer';
                div.innerHTML = html;
                document.body.appendChild(div);
            }
            
            // Attach cascading dropdown listeners
            attachRegionChangeListener();
            attachCityChangeListener();
            attachTownChangeListener();
            attachShopChangeListener();
            
            // Re-attach form submission listener
            const form = document.getElementById('shelfForm');
            if (form) {
                form.removeEventListener('submit', submitShelfForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitShelfForm();
                });
            }
            
            const modal = new bootstrap.Modal(document.getElementById('formModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Submit shelf form
function submitShelfForm() {
    const shelfId = document.getElementById('shelfId').value;
    const regionId = document.getElementById('shelfRegion').value;
    const cityId = document.getElementById('shelfCity').value;
    const townId = document.getElementById('shelfTown').value;
    const shopId = document.getElementById('shelfShop').value;
    const warehouseId = document.getElementById('shelfWarehouse').value;
    const code = document.getElementById('shelfCode').value.trim();
    const isActive = document.getElementById('shelfIsActive').checked;

    // Validation
    if (!regionId) {
        showAlert('Lütfen bir bölge seçin', 'error');
        document.getElementById('shelfRegion').focus();
        return;
    }

    if (!cityId) {
        showAlert('Lütfen bir şehir seçin', 'error');
        document.getElementById('shelfCity').focus();
        return;
    }

    if (!townId) {
        showAlert('Lütfen bir ilçe seçin', 'error');
        document.getElementById('shelfTown').focus();
        return;
    }

    if (!shopId) {
        showAlert('Lütfen bir mağaza seçin', 'error');
        document.getElementById('shelfShop').focus();
        return;
    }

    if (!warehouseId) {
        showAlert('Lütfen bir depo seçin', 'error');
        document.getElementById('shelfWarehouse').focus();
        return;
    }

    if (!code) {
        showAlert('Raf kodu boş olamaz', 'error');
        document.getElementById('shelfCode').focus();
        return;
    }

    const url = shelfId && parseInt(shelfId) > 0 ? `/Shelf/Update/${shelfId}` : '/Shelf/CreateJson';
    const formData = new FormData();
    formData.append('id', shelfId || '0');
    formData.append('code', code);
    formData.append('warehouseId', warehouseId);
    formData.append('isActive', isActive);

    fetch(url, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('formModal'));
            if (modal) modal.hide();
            
            showAlert(data.message || 'İşlem başarılı', 'success');
            // Reload page to reflect changes
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showAlert(data.message || 'Bir hata oluştu', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('İşlem sırasında bir hata oluştu', 'error');
    });
}

// Delete shelf (show confirmation modal)
function deleteShelf(shelfId) {
    fetch(`/Shelf/DeleteConfirm?id=${shelfId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('deleteModalContainer');
            if (container) {
                container.innerHTML = html;
            } else {
                const div = document.createElement('div');
                div.id = 'deleteModalContainer';
                div.innerHTML = html;
                document.body.appendChild(div);
            }
            
            currentShelfId = shelfId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete shelf
function confirmDeleteShelf() {
    const deleteShelfIdInput = document.getElementById('deleteShelfId');
    const shelfId = deleteShelfIdInput ? deleteShelfIdInput.value : currentShelfId;
    
    if (!shelfId) return;

    fetch(`/Shelf/DeleteJson/${shelfId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
            if (modal) modal.hide();
            
            showAlert(data.message || 'Raf başarıyla silindi', 'success');
            // Reload page to reflect changes
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showAlert(data.message || 'Bir hata oluştu', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Silme işlemi sırasında bir hata oluştu', 'error');
    });

    currentShelfId = null;
}

// Show alert
function showAlert(message, type) {
    if (typeof AlertModal !== 'undefined' && AlertModal.bootstrapModal) {
        if (type === 'success') {
            AlertModal.success(message);
        } else {
            AlertModal.error(message);
        }
    } else {
        alert(message);
    }
}
