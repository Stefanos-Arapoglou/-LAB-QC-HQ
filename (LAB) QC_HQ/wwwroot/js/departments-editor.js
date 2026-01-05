document.addEventListener("DOMContentLoaded", function () {

    let depIndex = document.querySelectorAll(".department-entry").length;

    const container = document.getElementById("departments-container");
    const addBtn = document.getElementById("add-department");

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
                    <option value="0">None</option>
                    <option value="1">Viewer</option>
                    <option value="2">Contributor</option>
                    <option value="3">Manager</option>
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

    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("remove-department")) {
            e.target.closest(".department-entry").remove();
            updateDepartmentOptions(); // 🔥 REQUIRED
        }
    });

    document.addEventListener("change", function (e) {
        if (e.target.name?.includes("DepartmentId")) {
            updateDepartmentOptions();
        }
    });

    // Initial run
    updateDepartmentOptions();
});
