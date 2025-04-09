const buttons = document.querySelectorAll(".action-btn");

const modal = document.getElementById("taskModal");
const closeBtn = document.querySelector(".modal-close");
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

function showToast(isSuccess, title, message) {
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
    await fetch("/Home/AddUserTask", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({ TaskTitle: taskTitle.value, TaskDescription: taskInput.value }),
    })
        .then((response) => response.json())
        .then((success) => {
            if (success) {
                showToast(true, 'Успех', 'Операция выполнена успешно');
            } else {
                showToast(false, 'Ошибка','Произошла ошибка');
            }
        })
        .catch((error) => {
            console.error("Ошибка:", error);
        });
}

async function getUserTask(isClosed) {
    await fetch("/Grid/GetUserTasks", {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({ TaskTitle: taskTitle.value, TaskDescription: taskInput.value }),
    })
        .then((response) => response.json())
        .then((success) => {
            if (success) {
                showToast(true, 'Успех', 'Операция выполнена успешно');
            } else {
                showToast(false, 'Ошибка', 'Произошла ошибка');
            }
        })
        .catch((error) => {
            console.error("Ошибка:", error);
        });
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

closeBtn.addEventListener("click", closeModal);
cancelBtn.addEventListener("click", closeModal);

submitBtn.addEventListener("click", function () {
    if (taskTitle.value.trim().length > 50) {
        showToast(
            true,
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
            true,
            "Неверно заполнена форма",
            "Количество символов должно быть не меньше 10"
        );
    }
});

