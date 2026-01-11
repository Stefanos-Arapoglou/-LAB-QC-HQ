// content-items.js

let itemIndex = 0;

function initItemsEditor(containerId, addButtonId) {
    const container = document.getElementById(containerId);
    const addButton = document.getElementById(addButtonId);

    if (!container) return;

    // If existing items exist, reuse their index count
    itemIndex = container.querySelectorAll(".item-entry").length;

    function updateIndexing() {
        const items = container.querySelectorAll(".item-entry");
        items.forEach((item, i) => {
            item.querySelector(".order-input").value = i;

            // Update name attributes so MVC binds correctly
            item.querySelectorAll("[name]").forEach(input => {
                input.name = input.name.replace(/Items\[\d+\]/, `Items[${i}]`);
            });
        });
        itemIndex = items.length;
    }

    function syncFields(entry) {
        const type = entry.querySelector(".item-type").value;
        const fileInput = entry.querySelector(".file-upload");
        const valueInput = entry.querySelector(".item-value");

        if (type === "File" || type === "Image") {
            fileInput.style.display = "";
            valueInput.disabled = true;
        } else {
            fileInput.style.display = "none";
            valueInput.disabled = false;
        }
    }

    function autoGrow(textarea) {
        textarea.style.height = "auto";
        textarea.style.height = textarea.scrollHeight + "px";
    }

    function wireEntry(entry) {
        entry.querySelector(".item-type")
            .addEventListener("change", () => syncFields(entry));

        const textArea = entry.querySelector(".item-value");
        textArea.addEventListener("input", () => autoGrow(textArea));
        autoGrow(textArea);

        syncFields(entry);
    }

    // Wire existing entries
    container.querySelectorAll(".item-entry").forEach(wireEntry);

    // Add new item
    if (addButton) {
        addButton.addEventListener("click", function () {
            const div = document.createElement("div");
            div.classList.add("item-entry", "mb-3", "border", "p-2", "rounded", "position-relative");

            div.innerHTML = `
                <input name="Items[${itemIndex}].ItemId" type="hidden" value="0" />

                <input name="Items[${itemIndex}].DisplayOrder"
                       class="form-control form-control-sm order-input d-inline w-auto me-2"
                       type="number" value="${itemIndex}" readonly />

                <input name="Items[${itemIndex}].ItemTitle"
                       type="text"
                       class="form-control mb-1"
                       placeholder="Item title / description"
                       required />

                <select name="Items[${itemIndex}].ItemType"
                        class="form-select mb-1 item-type">
                    <option value="Text">Text</option>
                    <option value="Image">Image</option>
                    <option value="Link">Link</option>
                    <option value="File">File</option>
                </select>

                <textarea name="Items[${itemIndex}].ItemValue"
                          class="form-control mb-1 item-value"
                          rows="1"></textarea>

                <input name="Items[${itemIndex}].FileUpload"
                       type="file"
                       class="form-control mb-1 file-upload" />

                <button type="button"
                        class="btn btn-outline-danger btn-sm remove-item position-absolute top-0 end-0 m-1">✖</button>
            `;

            container.appendChild(div);
            wireEntry(div);
            updateIndexing();
        });
    }

    // Remove item
    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("remove-item")) {
            e.target.closest(".item-entry")?.remove();
            updateIndexing();
        }
    });

    // Enable drag + drop if SortableJS is loaded
    if (typeof Sortable !== "undefined") {
        new Sortable(container, {
            animation: 150,
            onEnd: updateIndexing
        });
    }

    // Initialize index & fields
    updateIndexing();
}