const buttons = document.querySelectorAll(".action-btn");
const modal = document.getElementById("taskModal");

const cancelBtn = document.getElementById("cancelBtn");
const submitBtn = document.getElementById("submitBtn");
const taskInput = document.getElementById("taskInput");
const taskTitle = document.getElementById("taskTitle");

function openModal() {
    modal.style.display = "flex";
    setTimeout(() => {
        modal.classList.add("active");
    }, 10);
    taskInput.focus();
}

function closeModal() {
    modal.classList.remove("active");
    setTimeout(() => {
        modal.style.display = "none";
    }, 300);
    taskInput.value = "";
    taskTitle.value = "";
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

async function addUserTask() {
    const title = taskTitle.value.trim();
    const description = taskInput.value.trim();

    closeModal();

    try {
        const response = await fetch("/Tasks/AddTask/", {
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
}

buttons.forEach((button) => {
    button.addEventListener("click", function () {
        switch (this.id) {
            case "current-task-button":
                window.location.href = '/Tasks/MyTasks?IsClosed=false';
                break;
            case "new-task-button":
                openModal();
                break;
            case "closed-task-button":
                window.location.href = '/Tasks/MyTasks?IsClosed=true';
                break;
            default:
                break;
        }
    });
});

cancelBtn.addEventListener("click", closeModal);

submitBtn.addEventListener("click", function () {
    if (taskTitle.value.trim().length > 50) {
        showToast(
            "Неверно заполнена форма",
            "Количество символов в описании должно быть не больше 50"
        );
    } else if (
        taskInput.value.trim().length > 10 ||
        taskTitle.value.trim().length > 10
    ) {
        addUserTask();
    } else {
        showToast(
            "Неверно заполнена форма",
            "Количество символов должно быть не меньше 10"
        );
    }
});

