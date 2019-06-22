const actionsWrapper = document.getElementById('main_section');
const historyContainer = document.createElement('div');

createMarkup();

let intervalId = null;
let pause = true;
let currentTick = 0;
let robotTrack = [];
let ticks = [];
let currentProblemNumber = null;
let lastCommandPosition = 0;
let history = [];

function createMarkup() {
    const controlCenter = document.createElement('div');
    controlCenter.classList.add('control-center');

    const zeroRow = document.createElement('form');
    zeroRow.classList.add('row');
    zeroRow.addEventListener('submit', submitForm);

    const input = document.createElement('input');
    input.setAttribute('type', 'number');
    input.id = 'taskNumber';

    const firstRow = document.createElement('div');
    firstRow.classList.add('row');

    const prevButton = document.createElement('button');
    prevButton.textContent = '<';
    prevButton.addEventListener('click', prevTick);

    const nextButton = document.createElement('button');
    nextButton.textContent = '>';
    nextButton.addEventListener('click', nextTick);

    const playButton = document.createElement('button');
    playButton.textContent = '▶︎||';
    playButton.addEventListener('click', playPause);

    const resetButton = document.createElement('button');
    resetButton.classList.add('reset-button');
    resetButton.textContent = '↺';
    resetButton.addEventListener('click', reset);

    const secondRow = document.createElement('div');
    secondRow.classList.add('row');
    secondRow.classList.add('second-row');

    const prevFiveButton = document.createElement('button');
    prevFiveButton.textContent = '<<';
    prevFiveButton.addEventListener('click', prevFiveTick);

    const nextFiveButton = document.createElement('button');
    nextFiveButton.textContent = '>>';
    nextFiveButton.addEventListener('click', nextFiveTick);

    const historyContainer = document.createElement('div');
    historyContainer.classList.add('history-wrapper');

    zeroRow.appendChild(input);

    firstRow.appendChild(prevButton);
    firstRow.appendChild(playButton);
    firstRow.appendChild(nextButton);

    secondRow.appendChild(prevFiveButton);
    secondRow.appendChild(nextFiveButton);

    controlCenter.appendChild(firstRow);
    controlCenter.appendChild(secondRow);

    actionsWrapper.appendChild(zeroRow);
    actionsWrapper.appendChild(controlCenter);
    actionsWrapper.appendChild(resetButton);
    actionsWrapper.appendChild(historyContainer);
}


function playPause(e) {
    e.preventDefault();


    if (pause) {
        pause = false;
        intervalId = setInterval(() => {
            currentTick++;
            nextTick();
        }, 20)
    } else {
        stop();
    }
}

function nextTick(e) {
    saveImage();

    if (e) {
        e.preventDefault();
        currentTick++;
    }

    const gameObj = W();
    gameObj.Hi = false;
    try {
        createHistory();
        Wl();
        // renderHistory();
    } catch (e) {
        stop();
    } finally {
        gameObj.Hi = true;
    }
}

function nextFiveTick(e) {
    if (e) {
        e.preventDefault();
    }

    for (let i = 0; i < 5; i++) {
        currentTick++;
        nextTick();
    }
}

function prevTick(e) {
    if (e) {
        e.preventDefault();
    }

    currentTick--;

    const ctx = em(W());
    const image = ticks[currentTick - 1];
    ctx.putImageData(image, 0, 0)

}

function prevFiveTick(e) {
    if (e) {
        e.preventDefault();
    }

    currentTick = currentTick - 4;
    currentTick = currentTick < 0 ? 0 : currentTick;
    prevTick();
}

function reset(e) {
    e.preventDefault();
    stop();
    W().Pf.h(e);
    W().Hi = true;
    currentTick = 0;
    robotTrack = [];
    ticks = {};
    history = [];
    // renderHistory();
}

function stop() {
    pause = true;

    if (intervalId) {
        clearInterval(intervalId);
        intervalId = null;
    }
}

function addTrajectoryPoint(x, y) {
    const lastPoint = robotTrack[robotTrack.length - 1];
    if (!lastPoint || (lastPoint.x !== x || lastPoint.y !== y)) {
        robotTrack.push({x, y});
    }

    if (robotTrack.length > 1) {
        // drawTrack();
    }
}

function drawTrack() {
    const ctx = em(W());

    ctx.strokeStyle = "#0000ff";
    ctx.setLineDash([10, 10]);
    ctx.beginPath();

    const startPoint = robotTrack[0];
    ctx.moveTo(startPoint.x, startPoint.y);

    for (const point of robotTrack) {
        ctx.lineTo(point.x, point.y);
    }

    ctx.stroke();


}

function useProblem(number) {
    pause = true;
    currentTick = 0;
    robotTrack = [];

    mm(W(), window.files.desc[number].trim());
    nm(W(), window.files.sol[number].trim());
    setTimeout(() => {
        const prepare = rm(Tl(), W().Pf);
        prepare();
    }, 55);
}

function submitForm(e) {
    e.preventDefault();
    const selectedTask = document.getElementById('taskNumber').value;

    currentProblemNumber = selectedTask;
    useProblem(selectedTask);
}

function saveImage() {
    const canvas = fm(W());
    const ctx = em(W());
    let imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
    ticks[currentTick] = imageData;

    Object.keys(ticks).forEach(key => {
        if (currentTick - key > 50 ) {
            delete ticks[key];
        }
    });
}

function createHistory() {
    let solution = files.sol[currentProblemNumber];

    const nextCommand = solution[lastCommandPosition++];
    let afterCommand = solution[lastCommandPosition];

    if (afterCommand === '(') {
        lastCommandPosition++;
        while (solution[lastCommandPosition] !== ')') {
            afterCommand += solution[lastCommandPosition++];
        }
    }

    switch (nextCommand) {
        case 'W':
            history.push('↑ Двигаюсь вверх');
            break;
        case 'A':
            history.push('← Двигаюсь влево');
            break;
        case 'S':
            history.push('↓ Двигаюсь вниз');
            break;
        case 'D':
            history.push('→ Двигаюсь вправо');
            break;
        case 'Z':
            history.push('⧖ Жду...');
            break;
        case 'E':
            history.push('↻ Повернулся по часовой стрелке');
            break;
        case 'Q':
            history.push('↺ Повернулся против часовой стрелки');
            break;
        case 'F':
            history.push('♿︎ Применил Fast Wheels');
            break;
        case 'L':
            history.push('🍆 Начал использовать дрель');
            break;
        case 'R':
            history.push('🚨 Установил маяк');
            break;
        case 'B':
            afterCommand = afterCommand.replace(/[()]/g, '');
            history.push('🏗 Добавил манипулятор по координатам ' + afterCommand);
            break;
        case 'T':
            afterCommand = afterCommand.replace(/[()]/g, '');
            history.push('🛸 Телепоровался к маяку по координатам' + afterCommand);
            break;
        default:
            history.push('❓ Я не знаю, что это было')
    }
}

function renderHistory() {
    historyContainer.innerHTML = '';

    for (let i = 0; i < currentTick; i++ ) {
        const historyItem = document.createElement('div');
        historyItem.textContent = (i+1) + ": " + history[i];

        historyContainer.appendChild(historyItem);
    }
}
