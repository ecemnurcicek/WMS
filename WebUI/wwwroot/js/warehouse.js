// WareHouse Management JavaScript

let currentWareHouseId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeWareHouseManagement();
});

function initializeWareHouseManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new warehouse button
    const btnAddWareHouse = document.getElementById('btnAddWareHouse');
    if (btnAddWareHouse) {
        btnAddWareHouse.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Attach Region change listener for cascading dropdown
function attachRegionChangeListener() {
    const regionSelect = document.getElementById('wareHouseRegion');
    if (regionSelect) {
        regionSelect.addEventListener('change', function () {
            const regionId = this.value;
            const citySelect = document.getElementById('wareHouseCity');
            const townSelect = document.getElementById('wareHouseTown');
            const shopSelect = document.getElementById('wareHouseShop');
            
            // Reset city, town and shop dropdowns
            citySelect.innerHTML = '<option value="">-- Şehir Yükleniyor... --</option>';
            townSelect.innerHTML = '<option value="">-- Önce Şehir Seçiniz --</option>';
            shopSelect.innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
            
            if (regionId) {
                fetch(`/WareHouse/GetCitiesByRegion?regionId=${regionId}`)
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
    const citySelect = document.getElementById('wareHouseCity');
    if (citySelect) {
        citySelect.addEventListener('change', function () {
            const cityId = this.value;
            const townSelect = document.getElementById('wareHouseTown');
            const shopSelect = document.getElementById('wareHouseShop');
            
            // Reset town and shop dropdowns
            townSelect.innerHTML = '<option value="">-- İlçe Yükleniyor... --</option>';
            shopSelect.innerHTML = '<option value="">-- Önce İlçe Seçiniz --</option>';
            
            if (cityId) {
                fetch(`/WareHouse/GetTownsByCity?cityId=${cityId}`)
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
    const townSelect = document.getElementById('wareHouseTown');
    if (townSelect) {
        townSelect.addEventListener('change', function () {
            const townId = this.value;
            const shopSelect = document.getElementById('wareHouseShop');
            
            // Reset shop dropdown
            shopSelect.innerHTML = '<option value="">-- Mağaza Yükleniyor... --</option>';
            
            if (townId) {
                fetch(`/WareHouse/GetShopsByTown?townId=${townId}`)
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

// Open Create Modal
function openCreateModal() {
    fetch('/WareHouse/CreateForm')
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
            
            // Re-attach form submission listener
            const form = document.getElementById('wareHouseForm');
            if (form) {
                form.removeEventListener('submit', submitWareHouseForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitWareHouseForm();
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

// Edit warehouse
function editWareHouse(wareHouseId) {
    fetch(`/WareHouse/EditForm?id=${wareHouseId}`)
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
            
            // Re-attach form submission listener
            const form = document.getElementById('wareHouseForm');
            if (form) {
                form.removeEventListener('submit', submitWareHouseForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitWareHouseForm();
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

// Submit warehouse form
function submitWareHouseForm() {
    const wareHouseId = document.getElementById('wareHouseId').value;
    const regionId = document.getElementById('wareHouseRegion').value;
    const cityId = document.getElementById('wareHouseCity').value;
    const townId = document.getElementById('wareHouseTown').value;
    const shopId = document.getElementById('wareHouseShop').value;
    const name = document.getElementById('wareHouseName').value.trim();
    const isActive = document.getElementById('wareHouseIsActive').checked;

    // Validation
    if (!regionId) {
        showAlert('Lütfen bir bölge seçin', 'error');
        document.getElementById('wareHouseRegion').focus();
        return;
    }

    if (!cityId) {
        showAlert('Lütfen bir şehir seçin', 'error');
        document.getElementById('wareHouseCity').focus();
        return;
    }

    if (!townId) {
        showAlert('Lütfen bir ilçe seçin', 'error');
        document.getElementById('wareHouseTown').focus();
        return;
    }

    if (!shopId) {
        showAlert('Lütfen bir mağaza seçin', 'error');
        document.getElementById('wareHouseShop').focus();
        return;
    }

    if (!name) {
        showAlert('Depo adı boş olamaz', 'error');
        document.getElementById('wareHouseName').focus();
        return;
    }

    const url = wareHouseId && parseInt(wareHouseId) > 0 ? `/WareHouse/Update/${wareHouseId}` : '/WareHouse/CreateJson';
    const formData = new FormData();
    formData.append('id', wareHouseId || '0');
    formData.append('name', name);
    formData.append('shopId', shopId);
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

// Delete warehouse (show confirmation modal)
function deleteWareHouse(wareHouseId) {
    fetch(`/WareHouse/DeleteConfirm?id=${wareHouseId}`)
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
            
            currentWareHouseId = wareHouseId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete warehouse
function confirmDeleteWareHouse() {
    const deleteWareHouseIdInput = document.getElementById('deleteWareHouseId');
    const wareHouseId = deleteWareHouseIdInput ? deleteWareHouseIdInput.value : currentWareHouseId;
    
    if (!wareHouseId) return;

    fetch(`/WareHouse/DeleteJson/${wareHouseId}`, {
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
            
            showAlert(data.message || 'Depo başarıyla silindi', 'success');
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

    currentWareHouseId = null;
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
