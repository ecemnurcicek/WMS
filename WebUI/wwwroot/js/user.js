// User Management JavaScript

document.addEventListener('DOMContentLoaded', function () {
    const btnAddUser = document.getElementById('btnAddUser');
    if (btnAddUser) {
        btnAddUser.addEventListener('click', function () {
            openUserCreateModal();
        });
    }
});

// Open Create Modal
function openUserCreateModal() {
    fetch('/User/CreateForm')
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('userFormModalContainer');
            container.innerHTML = html;

            attachUserFormListener();

            const modal = new bootstrap.Modal(document.getElementById('userFormModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showUserAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Edit user
function editUser(userId) {
    fetch(`/User/EditForm?id=${userId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('userFormModalContainer');
            container.innerHTML = html;

            attachUserFormListener();

            const modal = new bootstrap.Modal(document.getElementById('userFormModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showUserAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Attach form submit listener
function attachUserFormListener() {
    const form = document.getElementById('userForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            submitUserForm();
        });
    }
}

// Submit user form
function submitUserForm() {
    const userId = document.getElementById('userId').value;
    const name = document.getElementById('userName').value.trim();
    const email = document.getElementById('userEmail').value.trim();
    const phone = document.getElementById('userPhone').value.trim();
    const password = document.getElementById('userPassword').value;
    const brandId = document.getElementById('userBrandId').value;
    const shopId = document.getElementById('userShopId').value;
    const roleId = document.getElementById('userRoleId').value;
    const isActive = document.getElementById('userIsActive').checked;

    // Validation
    if (!name) {
        showUserAlert('Ad Soyad boş olamaz', 'error');
        document.getElementById('userName').focus();
        return;
    }

    if (!email) {
        showUserAlert('E-posta boş olamaz', 'error');
        document.getElementById('userEmail').focus();
        return;
    }

    if (!roleId) {
        showUserAlert('Lütfen bir rol seçin', 'error');
        document.getElementById('userRoleId').focus();
        return;
    }

    const isEdit = userId && parseInt(userId) > 0;

    if (!isEdit && !password) {
        showUserAlert('Şifre boş olamaz', 'error');
        document.getElementById('userPassword').focus();
        return;
    }

    const url = isEdit ? `/User/Update/${userId}` : '/User/CreateJson';
    const formData = new FormData();
    formData.append('Name', name);
    formData.append('Email', email);
    formData.append('Phone', phone);
    formData.append('Password', password);
    formData.append('BrandId', brandId || '');
    formData.append('ShopId', shopId || '');
    formData.append('RoleId', roleId);
    formData.append('IsActive', isActive);

    fetch(url, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('userFormModal'));
            if (modal) modal.hide();

            showUserAlert(data.message || 'İşlem başarılı', 'success', function () {
                window.location.reload();
            });
        } else {
            showUserAlert(data.message || 'Bir hata oluştu', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showUserAlert('İşlem sırasında bir hata oluştu', 'error');
    });
}

// Delete user (show confirmation modal)
function deleteUser(userId) {
    fetch(`/User/DeleteConfirm?id=${userId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('userDeleteModalContainer');
            container.innerHTML = html;

            const modal = new bootstrap.Modal(document.getElementById('deleteUserModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showUserAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete
function confirmDeleteUser() {
    const userId = document.getElementById('deleteUserId').value;
    if (!userId) return;

    fetch(`/User/DeleteJson/${userId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteUserModal'));
            if (modal) modal.hide();

            showUserAlert(data.message || 'Kullanıcı başarıyla silindi', 'success', function () {
                window.location.reload();
            });
        } else {
            showUserAlert(data.message || 'Bir hata oluştu', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showUserAlert('Silme işlemi sırasında bir hata oluştu', 'error');
    });
}

// Show alert (uses AlertModal if available)
function showUserAlert(message, type, callback) {
    if (typeof AlertModal !== 'undefined' && AlertModal.bootstrapModal) {
        if (type === 'success') {
            AlertModal.success(message, callback);
        } else {
            AlertModal.error(message, callback);
        }
    } else {
        alert(message);
        if (callback) callback();
    }
}
