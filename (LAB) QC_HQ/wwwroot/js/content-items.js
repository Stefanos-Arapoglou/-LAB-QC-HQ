// content-items.js

// Keeps track of next item index (works for create or edit)
let itemIndex = 0;

function initItemsEditor(containerId, addButtonId) {
    const container = document.getElementById(containerId);
    const addButton = document.getElementById(addButtonId);

    if (!container || !addButton) return;

    // Initialize itemIndex based on existing items
    itemIndex = container.querySelectorAll(".item-entry").length;

    // Add new item
    addButton.addEventListener("click", function () {
        const div = document.createElement("div");
        div.classList.add("item-entry", "mb-3", "border", "p-2", "rounded");

        div.innerHTML = `
            <input name="Items[${itemIndex}].ItemTitle"
                   type="text"
                   class="form-control mb-1"
                   placeholder="Item title / description"
                   required />

            <select name="Items[${itemIndex}].ItemType" class="form-select mb-1">
                <option value="Text">Text</option>
                <option value="Link">Link</option>
                <option value="File">File</option>
            </select>

            <input name="Items[${itemIndex}].ItemValue"
                   type="text"
                   class="form-control mb-1"
                   placeholder="Text or link (not required for files)" />

            <input name="Items[${itemIndex}].FileUpload"
                   type="file"
                   class="form-control mb-1" />

            <button type="button" class="btn btn-outline-danger btn-sm remove-item">✖</button>
        `;
        container.appendChild(div);
        itemIndex++;
    });

    // Remove item
    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("remove-item")) {
            e.target.closest(".item-entry").remove();
        }
    });
}

// Optionally, allow dynamic generation of department options for other editors
function setDepartmentsOptions(optionsHtml) {
    window.departmentsOptions = optionsHtml;
}
