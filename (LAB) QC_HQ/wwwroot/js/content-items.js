/*NOTES
    This JS lines are used to manage items in a content. Specifically, the drag and drop functionallity, 
    creation, deletion and updating of items on client side. 
    
    MAIN USAGE: Views/Shared/_ItemsEditor.cshtml
    SEEN IN: All edit and create views that use items editor partial view

*/

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
        const fileUploadContainer = itemEntry.querySelector('.file-upload-container');
        const valueContainer = itemEntry.querySelector('.value-input-container');
        const previewArea = itemEntry.querySelector('.item-preview');

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

        // Hide preview area for new items
        if (previewArea) {
            previewArea.style.display = 'none';
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
        
        <!-- Title -->
        <div class="col-md-5">
            <input name="Items[${itemIndex}].ItemTitle"
                   class="form-control mb-2 item-title-input"
                   placeholder="Item title"
                   required />
        </div>
        
        <!-- Actions -->
        <div class="col-auto ms-auto">
            <button type="button" 
                    class="btn btn-sm btn-outline-danger remove-item-btn"
                    onclick="itemsEditor.removeItem(this)">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    </div>

    <!-- Preview Area (empty for new items) -->
    <div class="item-preview mt-2" style="display: none;">
    </div>

    <!-- Content Editing Fields (Always Visible) -->
    <div class="item-edit-fields mt-2">
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
    </div>
`;

        container.appendChild(newItem);
        itemIndex++;
        updateDisplayOrders();
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