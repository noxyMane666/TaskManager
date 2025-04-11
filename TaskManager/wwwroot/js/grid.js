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

async function apiRequest(url, method = 'GET', body = null) {
    try {
        const options = {
            method,
            headers: {
                'Content-Type': 'application/json',
            },
        };

        if (body) {
            options.body = JSON.stringify(body);
        }

        const response = await fetch(url, options);
        const data = await response.json();

        if (!response.ok || !data.success) {
            throw new Error(data.message || 'Ошибка запроса');
        }

        return data;
    } catch (error) {
        console.error(`[API ERROR] ${url}:`, error.message);
        throw error;
    }
}

async function updateTaskStatus(taskId, boolState) {
    try {
        await apiRequest('UpdateTaskState', 'POST', {
            Id: taskId,
            IsClosed: boolState
        });

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
    } catch (error) {
        console.error(error);

        const toggle = document.querySelector(`.status-toggle[data-task-id="${taskId}"]`);
        toggle.checked = !boolState;

        showToast('Ошибка', error.message || 'Ошибка обновления статуса');
    }
}

async function claimTaskUpdates(taskId) {
    const title = taskCardTitle.value.trim();
    const description = taskCardInput.value.trim();

    closeGridModal();

    try {
        await apiRequest('UpdateTask', 'POST', {
            Id: taskId,
            TaskTitle: title,
            TaskDescription: description
        });

        showToast('Успех', 'Задача успешно обновлена');
    } catch (error) {
        showToast('Ошибка', error.message || 'Произошла непредвиденная ошибка');
    }
}

async function deleteTask(taskId) {
    try {
        await apiRequest('DeleteTask', 'POST', { Id: taskId });
        showToast('Успех', 'Задача успешно удалена');
    } catch (error) {
        showToast('Ошибка', error.message || 'Ошибка при удалении задачи');
    }
}


document.querySelectorAll('.status-toggle').forEach(toggle => {
    toggle.addEventListener('change', async function () {
        const taskId = this.getAttribute('data-task-id');
        const isClosed = this.checked;

        await updateTaskStatus(taskId, isClosed);
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
cardSubmitBtn.addEventListener("click", async function () {
    if (taskCardTitle.value.trim().length > 50) {
        showToast(
            "Неверно заполнена форма",
            "Количество символов в описании должно быть не больше 50"
        );
    } else if (
        taskCardInput.value.trim().length > 10 ||
        taskCardTitle.value.trim().length > 10
    ) {
        await claimTaskUpdates();
    } else {
        showToast(
            "Неверно заполнена форма",
            "Количество символов должно быть не меньше 10"
        );
    }
});