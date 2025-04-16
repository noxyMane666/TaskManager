
function getToggleText(boolState) {
    return boolState ? "Завершена" : "Активна";
}

function updateTaskOnPage(taskId, newTitle, newDescription) {
    const taskElement = document.getElementById(`task-${taskId}`);

    if (taskElement) {
        setTimeout(() => {
            const titleElement = taskElement.querySelector('.title');
            const descElement = taskElement.querySelector('.text');

            titleElement.textContent = newTitle;
            descElement.value = `"${newDescription}"`;
        }, 300);
    }
}

function removeTaskFromPage(taskId) {
    const taskElement = document.getElementById(`task-${taskId}`);

    if (taskElement) {
        setTimeout(() => {
            taskElement.remove();
        }, 300);
    }
}

async function updateTaskStatus(taskId, boolState) {
    try {
        await apiPostRequest(API_ENDPOINTS.UPDATE_TASK_STATE, 'POST', {
            Id: taskId,
            IsClosed: boolState
        });

        const taskCard = document.getElementById(`task-${taskId}`);
        const toggle = document.querySelector(`.status-toggle[data-task-id="${taskId}"]`);
        const toggleText = toggle.closest('.task-status')
            .querySelector('span:not(.slider)');

        toggleText.textContent = getToggleText(boolState);

        setTimeout(() => {
            taskCard.remove();
        }, 300);
    } catch (error) {
        console.error(error);

        const toggle = document.querySelector(`.status-toggle[data-task-id="${taskId}"]`);
        toggle.checked = !boolState;

        showToast('Ошибка', 'Ошибка обновления статуса');
    }
}

async function submitTaskUpdates(taskId) {
    const id = taskId.id
    const title = taskId.title;
    const description = taskId.description;

    try {
        const data = await apiPostRequest(API_ENDPOINTS.UPDATE_TASK, 'POST', {
            Id: id,
            TaskTitle: title,
            TaskDescription: description
        });

        updateTaskOnPage(taskId.id, taskId.title, taskId.description);
        return data.updatedTask
    } catch (error) {
        throw error;
    }
}
async function deleteTask(taskId) {
    try {
        await apiPostRequest(API_ENDPOINTS.DELETE_TASK, 'POST', { Id: taskId.Id });

        removeTaskFromPage(taskId.Id);
    } catch (error) {
        throw error;
    }
}


function openGridModal(taskElement) {
    const gridModal = document.getElementById("gridTaskModal");
    const cardSubmitBtn = document.getElementById("cardSubmitBtn");
    const cardCancelBtn = document.getElementById("cardCancelBtn");
    const cardDeleteBtn = document.getElementById("cardDeleteBtn");
    const taskId = parseInt(taskElement.id.replace('task-', ''), 10);

    let title = taskElement.querySelector('.title').textContent;
    let description = taskElement.querySelector('.text').value;
    let taskCardTitle = document.getElementById("gridTaskTitle");
    let taskCardInput = document.getElementById("gridTaskInput");

    function cleanUp() {
        cardSubmitBtn.removeEventListener('click', handleSubmit);
        cardCancelBtn.removeEventListener('click', closeGridModal);
        cardDeleteBtn.removeEventListener('click', deleteTask);
    }

    function closeGridModal() {
        cleanUp() 
        gridModal.classList.remove("active");
        setTimeout(() => {
            gridModal.style.display = "none";
        }, 300);
    }

    async function handleSubmit() {
        closeGridModal();
        try {
            if (taskCardTitle.value.trim() !== title.trim() ||
                taskCardInput.value.trim() !== description.replace(/^"|"$/g, '').trim()) {
                const updatedTask = await submitTaskUpdates({
                    id: taskId,
                    title: taskCardTitle.value,
                    description: taskCardInput.value
                });

                title = updatedTask.title
                description = updatedTask.description

                showToast('Успех', 'Данные успешно сохранены');
            }
        } catch (error) {
            console.error(error);
            showToast('Ошибка', `Ошибка при обновлении задачи ${taskId}`);
        }
    }

    async function handleDelete() {
        closeGridModal();
        try {
            await deleteTask({
                Id: taskId
            });

            showToast('Успех', 'Задача успешно удалена');
        } catch (error) {
            console.error(error)
            showToast('Ошибка', `Ошибка при удалении задачи ${taskId}`);
        }
    }

    taskCardTitle.value = title;
    taskCardInput.value = description.replace(/^"|"$/g, '');

    gridModal.style.display = "flex";
    setTimeout(() => {
        gridModal.classList.add("active");
    }, 10);
    taskCardTitle.focus();

    cardCancelBtn.addEventListener('click', closeGridModal);
    cardDeleteBtn.addEventListener('click', handleDelete);
    cardSubmitBtn.addEventListener('click', handleSubmit)

}

function initTaskListeners() {
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
}

document.addEventListener('DOMContentLoaded', initTaskListeners);