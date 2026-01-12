/*NOTES
    This JS lines are used to manage Departments and clearance level, when creating or editing Content. 
    It allows dynamic addition and removal of department entries, ensuring no duplicate departments are selected, etc
    
    MAIN USAGE: Views/Shared/_DepartmentsEditor.cshtml
    SEEN IN: All edit and create views that use Departments Editor partial view.

*/



document.addEventListener("DOMContentLoaded", function () {

    let depIndex = document.querySelectorAll(".department-entry").length;

    const container = document.getElementById("departments-container");
    const addBtn = document.getElementById("add-department");

    // Update Department select options to prevent duplicates
    function updateDepartmentOptions() {
        const selects = container.querySelectorAll(
            'select[name$=".DepartmentId"]'
        );

        const selectedValues = Array.from(selects)
            .map(s => s.value)
            .filter(v => v !== "");

        selects.forEach(select => {
            const currentValue = select.value;

            Array.from(select.options).forEach(option => {
                if (option.value === "") return;

                option.disabled =
                    option.value !== currentValue &&
                    selectedValues.includes(option.value);
            });
        });
    }

    // to Add X buttons to EXISTING departments (for Edit mode)
    function addRemoveButtonsToExistingDepartments() {
        const existingEntries = container.querySelectorAll(".department-entry");
        existingEntries.forEach((entry, index) => {
            // Check if button already exists
            if (!entry.querySelector(".remove-department")) {
                // Create X button
                const removeBtn = document.createElement("button");
                removeBtn.type = "button";
                removeBtn.className = "btn btn-outline-danger btn-sm ms-2 remove-department";
                removeBtn.textContent = "✖";
                removeBtn.setAttribute("data-index", index);

                // Add hidden field for marking removal
                const hiddenField = document.createElement("input");
                hiddenField.type = "hidden";
                hiddenField.name = `Departments[${index}].__MarkedForRemoval`;
                hiddenField.id = `Departments[${index}]__MarkedForRemoval`;
                hiddenField.value = "false";

                // Add hidden field to entry
                entry.appendChild(hiddenField);
                // Add X button to entry
                entry.appendChild(removeBtn);
            }
        });
    }

    // Initialize remove buttons for existing departments
    addRemoveButtonsToExistingDepartments();

    // Add new department entry
    addBtn?.addEventListener("click", function () {

        const template = `
            <div class="department-entry mb-2 d-flex gap-2 align-items-center">
                <select name="Departments[${depIndex}].DepartmentId"
                        class="form-select w-50">
                    <option value="">-- Select Department --</option>
                    ${window.departmentsOptions}
                </select>

                <select name="Departments[${depIndex}].ClearanceLevelRequired"
                        class="form-select w-25">
                    <option value="0">0 - No Access</option>
                    <option value="1">1 - Basic Access</option>
                    <option value="2">2 - Advanced Access</option>
                    <option value="3">3 - Full Access</option>
                </select>

                <button type="button"
                        class="btn btn-outline-danger btn-sm remove-department">
                    ✖
                </button>
            </div>
        `;

        container.insertAdjacentHTML("beforeend", template);
        depIndex++;

        updateDepartmentOptions(); // 🔥 REQUIRED
    });

    // Remove department entry
    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("remove-department")) {
            e.target.closest(".department-entry").remove();
            updateDepartmentOptions(); // 🔥 REQUIRED
        }
    });

    // Update options on change
    document.addEventListener("change", function (e) {
        if (e.target.name?.includes("DepartmentId")) {
            updateDepartmentOptions();
        }
    });

    // Initial run
    updateDepartmentOptions();
});
