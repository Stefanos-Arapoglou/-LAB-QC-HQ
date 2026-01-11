// content-items.js - Enhanced Items Editor with Drag & Drop

const itemsEditor = (function () {
    let itemIndex = 0;
    let sortableInstance = null;
    const containerId = 'items-container';
    const addButtonId = 'add-item-btn';

    // Initialize the items editor
    function init() {
        const container = document.getElementById(containerId);
        const addButton = document.getElementById(addButtonId);

        if (!container) {
            console.error('Items container not found');
            return;
        }

        // Initialize index from existing items
        itemIndex = container.querySelectorAll('.item-entry').length;

        // Initialize drag & drop
        initSortable();

        // Wire up add button
        if (addButton) {
            addButton.addEventListener('click', addNewItem);
        }

        // Update display orders initially
        updateDisplayOrders();
    }

    // Initialize SortableJS for drag & drop
    function initSortable() {
        const container = document.getElementById(containerId);
        if (typeof Sortable === 'undefined') {
            console.warn('SortableJS not loaded. Drag & drop disabled.');
            return;
        }

        sortableInstance = new Sortable(container, {
            animation: 150,
            handle: '.sortable-handle',
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            onEnd: function () {
                updateDisplayOrders();
            }
        });
    }

    // Update display orders after reordering
    function updateDisplayOrders() {
        const container = document.getElementById(containerId);
        const items = container.querySelectorAll('.item-entry');

        items.forEach((item, index) => {
            const orderInput = item.querySelector('.display-order-input');
            const orderBadge = item.querySelector('.order-badge');

            if (orderInput) {
                orderInput.value = index;
            }

            if (orderBadge) {
                orderBadge.textContent = '#' + index;
            }

            // Update name attributes for model binding
            updateItemNames(item, index);
        });

        itemIndex = items.length;
    }

    // Update name attributes for proper model binding
    function updateItemNames(itemElement, index) {
        const inputs = itemElement.querySelectorAll('[name]');
        inputs.forEach(input => {
            const oldName = input.name;
            const newName = oldName.replace(/Items\[\d+\]/, `Items[${index}]`);
            input.name = newName;
        });
    }

    // Handle item type change
    function handleTypeChange(selectElement) {
        const itemEntry = selectElement.closest('.item-entry');
        const type = selectElement.value;
        const typeHidden = itemEntry.querySelector('.item-type-hidden');
        const valueTextarea = itemEntry.querySelector('.item-value-textarea');
        const fileUploadContainer = itemEntry.querySelector('.file-upload-container');
        const valueContainer = itemEntry.querySelector('.value-input-container');
        const displayArea = itemEntry.querySelector('.item-content-display');

        // Update hidden type field
        if (typeHidden) {
            typeHidden.value = type;
        }

        // Show/hide appropriate fields
        if (type === 'Image' || type === 'File') {
            if (fileUploadContainer) fileUploadContainer.style.display = 'block';
            if (valueContainer) valueContainer.style.display = 'none';
        } else {
            if (fileUploadContainer) fileUploadContainer.style.display = 'none';
            if (valueContainer) valueContainer.style.display = 'block';
        }

        // Update display area preview
        updateDisplayPreview(itemEntry);
    }

    // Update the display preview area
    function updateDisplayPreview(itemEntry) {
        const displayArea = itemEntry.querySelector('.item-content-display');
        const type = itemEntry.querySelector('.item-type-select').value;
        const title = itemEntry.querySelector('.item-title-input').value;
        const value = itemEntry.querySelector('.item-value-textarea')?.value || '';

        let html = '';

        switch (type) {
            case 'Text':
                html = `<div class="text-content p-2 bg-light rounded">${value || 'No content'}</div>`;
                break;
            case 'Link':
                html = `<div class="link-content p-2 bg-light rounded">
                          <i class="bi bi-link-45deg"></i>
                          <a href="${value}" target="_blank">${value || 'No URL'}</a>
                        </div>`;
                break;
            case 'Image':
                html = `<div class="image-content text-center text-muted">No image uploaded</div>`;
                break;
            case 'File':
                html = `<div class="file-content p-2 bg-light rounded">
                          <i class="bi bi-file-earmark"></i>
                          <span class="text-muted">No file uploaded</span>
                        </div>`;
                break;
        }

        if (displayArea) {
            displayArea.innerHTML = html;
        }
    }

    // Add new item
    function addNewItem() {
        const container = document.getElementById(containerId);
        const template = document.getElementById('item-template');

        if (!template) {
            console.error('Item template not found');
            return;
        }

        const newItem = document.createElement('div');
        newItem.className = 'item-entry mb-3 border p-3 rounded position-relative sortable-item';
        newItem.setAttribute('data-item-id', '0');
        newItem.setAttribute('data-display-order', itemIndex);

        newItem.innerHTML = `
            <!-- Drag Handle -->
            <div class="sortable-handle position-absolute top-0 start-0 m-1" style="cursor: move;">
                <i class="bi bi-grip-vertical text-muted"></i>
            </div>

            <!-- Hidden Fields -->
            <input type="hidden" name="Items[${itemIndex}].ItemId" class="item-id-input" value="0" />
            <input type="hidden" name="Items[${itemIndex}].DisplayOrder" class="display-order-input" value="${itemIndex}" />
            <input type="hidden" name="Items[${itemIndex}].ItemType" class="item-type-hidden" value="Text" />

            <div class="row">
                <!-- Order Badge -->
                <div class="col-auto">
                    <span class="badge bg-secondary order-badge">#${itemIndex}</span>
                </div>
                
                <!-- Title -->
                <div class="col-md-5">
                    <input name="Items[${itemIndex}].ItemTitle"
                           class="form-control mb-2 item-title-input"
                           placeholder="Item title"
                           required />
                </div>
                
                <!-- Type Selector -->
                <div class="col-md-3">
                    <select name="Items[${itemIndex}].ItemType" 
                            class="form-select mb-2 item-type-select" 
                            onchange="itemsEditor.handleTypeChange(this)">
                        <option value="Text">Text</option>
                        <option value="Link">Link</option>
                        <option value="Image">Image</option>
                        <option value="File">File</option>
                    </select>
                </div>
                
                <!-- Actions -->
                <div class="col-auto ms-auto">
                    <button type="button" 
                            class="btn btn-sm btn-outline-primary me-1 edit-item-btn"
                            onclick="itemsEditor.editItem(this)">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button type="button" 
                            class="btn btn-sm btn-outline-danger remove-item-btn"
                            onclick="itemsEditor.removeItem(this)">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </div>

            <!-- Content Display Area -->
            <div class="item-content-display mt-2">
                <div class="text-content p-2 bg-light rounded">No content</div>
            </div>

            <!-- Editing Fields (Initially Hidden) -->
            <div class="item-edit-fields mt-2" style="display: none;">
                <!-- Value Input -->
                <div class="mb-2 value-input-container">
                    <textarea name="Items[${itemIndex}].ItemValue"
                              class="form-control item-value-textarea"
                              rows="3"
                              placeholder="Enter content or URL">
                    </textarea>
                </div>

                <!-- File Upload (hidden initially) -->
                <div class="mb-2 file-upload-container" style="display: none;">
                    <input type="file" 
                           name="Items[${itemIndex}].FileUpload"
                           class="form-control file-upload-input" />
                </div>

                <!-- Save/Cancel Buttons -->
                <div class="d-flex justify-content-end gap-2">
                    <button type="button" 
                            class="btn btn-sm btn-success save-edit-btn"
                            onclick="itemsEditor.saveEdit(this)">
                        Save
                    </button>
                    <button type="button" 
                            class="btn btn-sm btn-secondary cancel-edit-btn"
                            onclick="itemsEditor.cancelEdit(this)">
                        Cancel
                    </button>
                </div>
            </div>
        `;

        container.appendChild(newItem);
        itemIndex++;
        updateDisplayOrders();
    }

    // Edit item
    function editItem(button) {
        const itemEntry = button.closest('.item-entry');
        const editFields = itemEntry.querySelector('.item-edit-fields');
        const displayArea = itemEntry.querySelector('.item-content-display');

        // Show edit fields, hide display
        if (editFields) editFields.style.display = 'block';
        if (displayArea) displayArea.style.display = 'none';

        // Hide edit button
        button.style.display = 'none';
    }

    // Save edit
    function saveEdit(button) {
        const itemEntry = button.closest('.item-entry');
        const editFields = itemEntry.querySelector('.item-edit-fields');
        const displayArea = itemEntry.querySelector('.item-content-display');
        const editBtn = itemEntry.querySelector('.edit-item-btn');
        const type = itemEntry.querySelector('.item-type-select').value;

        // For file uploads, handle the file input
        if (type === 'Image' || type === 'File') {
            const fileInput = itemEntry.querySelector('.file-upload-input');
            if (fileInput && fileInput.files.length > 0) {
                // File will be handled on server side
                console.log('File selected:', fileInput.files[0].name);
            }
        }

        // Update display preview
        updateDisplayPreview(itemEntry);

        // Hide edit fields, show display
        if (editFields) editFields.style.display = 'none';
        if (displayArea) displayArea.style.display = 'block';
        if (editBtn) editBtn.style.display = '';
    }

    // Cancel edit
    function cancelEdit(button) {
        const itemEntry = button.closest('.item-entry');
        const editFields = itemEntry.querySelector('.item-edit-fields');
        const displayArea = itemEntry.querySelector('.item-content-display');
        const editBtn = itemEntry.querySelector('.edit-item-btn');

        // Hide edit fields, show display
        if (editFields) editFields.style.display = 'none';
        if (displayArea) displayArea.style.display = 'block';
        if (editBtn) editBtn.style.display = '';
    }

    // Remove item
    function removeItem(button) {
        if (!confirm('Are you sure you want to remove this item?')) {
            return;
        }

        const itemEntry = button.closest('.item-entry');
        if (itemEntry) {
            itemEntry.remove();
            updateDisplayOrders();
        }
    }

    // Clear file selection
    function clearFile(button) {
        const itemEntry = button.closest('.item-entry');
        const fileInput = itemEntry.querySelector('.file-upload-input');
        const hiddenValue = itemEntry.querySelector('.item-value-hidden');

        if (fileInput) {
            fileInput.value = '';
        }

        if (hiddenValue) {
            hiddenValue.value = '';
        }

        button.closest('small').innerHTML = 'No file selected';
    }

    // Public API
    return {
        init: init,
        handleTypeChange: handleTypeChange,
        editItem: editItem,
        saveEdit: saveEdit,
        cancelEdit: cancelEdit,
        removeItem: removeItem,
        clearFile: clearFile,
        addNewItem: addNewItem,
        updateDisplayOrders: updateDisplayOrders
    };
})();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    itemsEditor.init();
});

// Export for global access
window.itemsEditor = itemsEditor;