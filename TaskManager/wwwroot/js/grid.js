async function apiRequest(url, method = 'POST', body = null) {
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
        console.error(`Api error ${url}:`, error.message);
        throw error;
    }
}

async function updateTaskStatus(taskId, boolState) {
    try {
        await apiRequest('UpdateTaskState', {
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

        showToast('Ошибка', 'Ошибка обновления статуса');
    }
}

async function submitTaskUpdates(taskId) {
    const id = taskId.id
    const title = taskId.title;
    const description = taskId.description;

    try {
        const updateRequest = await apiRequest('UpdateTask', {
            Id: id,
            TaskTitle: title,
            TaskDescription: description
        });

        const data = await updateRequest.json();

        if (!data.success) {
            throw new Error(data.message || 'Ошибка запроса');
        }

        showToast('Успех', 'Данные успешно сохранены');
    } catch (error) {
        console.error(error);
        showToast('Ошибка', 'Произошла непредвиденная ошибка');
    }
}

function getToggleText(boolState) {
    return boolState ? "Завершена" : "Активна";
}

function openGridModal(taskElement) {
    const gridModal = document.getElementById("gridTaskModal");
    const taskCardTitle = document.getElementById("gridTaskTitle");
    const taskCardInput = document.getElementById("gridTaskInput");
    const cardSubmitBtn = document.getElementById("cardSubmitBtn");
    const cardCancelBtn = document.getElementById("cardCancelBtn");
    //const cardDeleteBtn = document.getElementById("cardDeleteBtn");

    const taskId = taskElement.dataset.taskId;
    const title = taskElement.querySelector('.title').textContent;
    const description = taskElement.querySelector('.text').value;

    function handleSubmit() {
        console.log(1212121)
        if (taskCardTitle.value.trim() !== title.trim() ||
            taskCardInput.value.trim() !== description.replace(/^"|"$/g, '').trim()) {
            console.log(898989)
            submitTaskUpdates({
                id: taskId,
                title: taskCardTitle.value,
                description: taskCardInput.value
            });
        }

        cleanUp();
    }

    function cleanUp() {
        cardSubmitBtn.removeEventListener('click', handleSubmit);
        cardCancelBtn.removeEventListener('click', closeGridModal);
        //cardDeleteBtn.removeEventListener('click', deleteTask);
        //gridModal.removeEventListener('click', handleModalClick);
    }

    function closeGridModal() {
        cleanUp() 
        gridModal.classList.remove("active");
        setTimeout(() => {
            gridModal.style.display = "none";
        }, 300);
    }

    taskCardTitle.value = title;
    taskCardInput.value = description.replace(/^"|"$/g, '');

    gridModal.style.display = "flex";
    setTimeout(() => {
        gridModal.classList.add("active");
    }, 10);
    taskCardTitle.focus();

    cardCancelBtn.addEventListener('click', closeGridModal);
    //cardDeleteBtn.addEventListener('click', deleteTask);
    cardSubmitBtn.addEventListener('click', handleSubmit)

}

//async function deleteTask(taskId) {
//    try {
//        await apiRequest('DeleteTask', 'POST', { Id: taskId });
//        showToast('Успех', 'Задача успешно удалена');
//    } catch (error) {
//        showToast('Ошибка', error.message || 'Ошибка при удалении задачи');
//    }
//}

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