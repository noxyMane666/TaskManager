const API_ENDPOINTS = {
    ADD_TASK: '/Tasks/AddTask/',
    UPDATE_TASK_STATE: 'UpdateTaskState',
    UPDATE_TASK: 'UpdateTask',
    DELETE_TASK: 'DeleteTask'
}

async function apiPostRequest(url, method = 'POST', body = null) {
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

        if (!response.ok) {
            throw new Error(`HTTP ошибка: ${response.status}`);
        }

        const data = await response.json();

        if (!data.success) {
            throw new Error(data.message || 'Ошибка запроса');
        }

        return data;
    } catch (error) {
        throw error;
    }
}

function showToast(title, message) {
    toastTitle.textContent = title;
    toastMessage.textContent = message;

    toastNotification.style.display = "block";
    setTimeout(() => {
        toastNotification.classList.add("show");
    }, 10);
    setTimeout(() => {
        toastNotification.classList.remove("show");
        setTimeout(() => {
            toastNotification.style.display = "none";
        }, 500);
    }, 2000);
}

async function addUserTask(task) {
    const title = task.taskTitle;
    const description = task.taskDescription;

    try {
        const data = await apiPostRequest(API_ENDPOINTS.ADD_TASK, 'POST', {
            TaskTitle: title,
            TaskDescription: description
        });

        return data.updatedTask
    } catch (error) {
        throw error;
    }
}

function openHomeModal() {
    const modal = document.getElementById("taskModal");
    const cancelBtn = document.getElementById("cancelBtn");
    const submitBtn = document.getElementById("submitBtn");
 
    function cleanUp() {
        submitBtn.removeEventListener('click', handleCreatedTask);
        cancelBtn.removeEventListener('click', closeModal);
    }

    function closeModal() {
        cleanUp()
        setTimeout(() => {
            taskInput.value = "";
            taskTitle.value = "";
            modal.style.display = "none";
        }, 300);
    }

    async function handleCreatedTask() {
        const title = taskTitle.value;
        const description = taskInput.value;

        if (title.length > 50) {
            showToast(
                "Ошибка",
                "Заголовок не должен превышать 50 символов",
                'error'
            );
            return;
        }

        if (title.length < 10 || description.length < 10) {
            showToast("Ошибка", "Минимальная длина - 10 символов");
            return;
        }


        closeModal()
        try {
            await addUserTask({
                taskTitle: title.trim(),
                taskDescription: description.trim()
            });

            showToast("Успех", "Задача успешно добавлена");
        } catch (error) {
            console.error("Ошибка:", error);
        }
    }

    modal.style.display = "flex";
    setTimeout(() => {
        modal.classList.add("active");
    }, 100);
    taskInput.focus();

    cancelBtn.addEventListener("click", closeModal);
    submitBtn.addEventListener("click", handleCreatedTask);
}

function initHomeListeners() {
    const buttons = document.querySelectorAll(".action-btn");
    buttons.forEach((button) => {
        button.addEventListener("click", function () {
            switch (this.id) {
                case "current-task-button":
                    window.location.href = '/Tasks/MyTasks?IsClosed=false';
                    break;
                case "new-task-button":
                    openHomeModal();
                    break;
                case "closed-task-button":
                    window.location.href = '/Tasks/MyTasks?IsClosed=true';
                    break;
                default:
                    break;
            }
        });
    });
}

document.addEventListener('DOMContentLoaded', initHomeListeners);

