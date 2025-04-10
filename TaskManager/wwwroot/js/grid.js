const gridModal = document.getElementById("gridTaskModal");
const taskCardTitle = document.getElementById("gridTaskTitle");
const taskCardInput = document.getElementById("gridTaskInput");
const cardSubmitBtn = document.getElementById("cardSubmitBtn");
const cardCancelBtn = document.getElementById("cardCancelBtn");
const cardDeleteBtn = document.getElementById("cardDeleteBtn");

function getToggleText(boolState) {
    return boolState ? "Завершена" : "Активна";
}

function openGridModal(taskElement) {
    const title = taskElement.querySelector('.title').textContent;
    const description = taskElement.querySelector('.text').value;

    taskCardTitle.value = title;
    taskCardInput.value = description.replace(/^"|"$/g, '');

    gridModal.style.display = "flex";
    setTimeout(() => {
        gridModal.classList.add("active");
    }, 10);
    taskCardTitle.focus();
}

function closeGridModal() {
    gridModal.classList.remove("active");
    setTimeout(() => {
        gridModal.style.display = "none";
    }, 300);
}

async function updateTaskStatus(taskId, boolState) {
    await fetch('UpdateTaskState', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: taskId,
            IsClosed: boolState
        })
    }).then(response => {
        if (!response.ok) {
            throw new error('Ошибка обновления статуса');
        }
        return response.json();
    }).then(data => {
        if (data.success) {
            const taskCard = document.getElementById(`task-${taskId}`);
            const toggle = document.querySelector(`.status-toggle[data-task-id="${taskId}"]`);
            const toggleText = toggle.closest('.task-status').querySelector('span:not(.slider)');

            toggleText.textContent = getToggleText(boolState);

            taskCard.style.transition = 'opacity 0.3s ease-out, transform 0.3s ease-out';
            taskCard.style.opacity = '0';
            taskCard.style.transform = 'translateX(-20px)';

            setTimeout(() => {
                taskCard.remove();
            }, 300); 

        }
        else {
            throw new Error(`Задача с id ${taskId} не найдена`);
        }
    }).catch(error => {
        console.error(error);

        const toggle = document.querySelector(`.status-toggle[data-task-id="${taskId}"]`);
        toggle.checked = !boolState;
    });
};

async function claimTaskUpdates() {
    const title = taskCardTitle.value.trim();
    const description = taskCardInput.value.trim();

    closeGridModal();

    try
    {
        const response = await fetch("UpdateTask", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                TaskTitle: title,
                TaskDescription: description
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Ошибка сервера');
        }

        const result = await response.json();

        if (result.success) {
            showToast('Успех', 'Задача успешно добавлена');
        } else {
            showToast('Ошибка', result.message || 'Не удалось добавить задачу');
        }
    } catch (error) {
        console.error("Ошибка:", error);
        showToast('Ошибка', error.message || 'Произошла непредвиденная ошибка');
    }
};

document.querySelectorAll('.status-toggle').forEach(toggle => {
    toggle.addEventListener('change', function () {
        const taskId = this.getAttribute('data-task-id');
        const isClosed = this.checked;

        updateTaskStatus(taskId, isClosed);
    });
});

document.querySelectorAll('.task').forEach(task => {
    task.addEventListener('click', function (e) {
        if (!e.target.closest('.task-status')) {
            openGridModal(this);
        }
    });
});

cardCancelBtn.addEventListener("click", closeGridModal);
cardSubmitBtn.addEventListener("click", function () {
    if (taskTitle.value.trim().length > 50) {
        showToast(
            "Неверно заполнена форма",
            "Количество символов в описании должно быть не больше 50"
        );
    } else if (
        taskCardInput.value.trim().length > 10 ||
        taskCardTitle.value.trim().length > 10
    ) {
        addUserTask();
    } else {
        showToast(
            "Неверно заполнена форма",
            "Количество символов должно быть не меньше 10"
        );
    }
});