function getToggleText(boolState) {
    return boolState ? "Завершена" : "Активна";
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

const toggleButtons = document.querySelectorAll('.status-toggle').forEach(toggle => {
    toggle.addEventListener('change', function () {
        const taskId = this.getAttribute('data-task-id');
        const isClosed = this.checked;

        updateTaskStatus(taskId, isClosed);
    });
});