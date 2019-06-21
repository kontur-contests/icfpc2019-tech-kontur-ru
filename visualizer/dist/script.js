const actionsWrapper = document.getElementById('main_section');

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



let intervalId = null;
let pause = true;
let currentTick = 0;
let robotTrack = [];

function playPause(e) {
    e.preventDefault();


    if (pause) {
        pause = false;
        intervalId = setInterval(() => {
            currentTick++;
            nextTick();
        }, 30)
    } else {
        stop();
    }
}

function nextTick(e) {
    if (e) {
        e.preventDefault();
        currentTick++;
    }

    const gameObj = W();
    gameObj.Hi = false;
    try {
        Wl();
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

    robotTrack = [];

    W().Pf.h(e);

    setTimeout(() => {
        for (let i = 0; i < currentTick; i++) {
            nextTick();
        }
    }, 50);


    currentTick--;
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
        drawTrack();
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

    useProblem(selectedTask);
}

