const actionsWrapper = document.getElementById('main_section');

let currentProblemNumber = null;

const canvas = document.getElementById('canvas');
const ctx = canvas.getContext("2d");
const colors = {
    darkGrey: "#666",
    lightGrey: "#eaeaea",
    outerPoint: '#000000',
    innerPoint: '#00ff00',
};
let config;

createMarkup();
initialCanvas();

function createMarkup() {
    const zeroRow = document.createElement('form');
    zeroRow.classList.add('row');
    zeroRow.addEventListener('submit', submitForm);

    const input = document.createElement('input');
    input.setAttribute('type', 'number');
    input.id = 'taskNumber';

    zeroRow.appendChild(input);

    actionsWrapper.appendChild(zeroRow);
}





function submitForm(e) {
    e.preventDefault();

    const selectedTask = document.getElementById('taskNumber').value;

    currentProblemNumber = selectedTask;
    useProblem(selectedTask);
}

function useProblem(number) {
    const map = parseMap(window.files.desc[number].trim());
    config = calcMapSize(map);
    initialCanvas();
    renderMap(map);

    const points = parsePoints(window.files.cond[number].trim());
    renderPoints(points);
}

function parseMap(str) {
    const [ pointsStr, ...rest ] = str.split('#');
    const points = getAllPoints(pointsStr);

    return {
        edges: points,
    }
}

function parsePoints(solution) {
    const [_, inner, outer] = solution.split('#');

    const innerPoints = getAllPoints(inner);
    const outerPoints = getAllPoints(outer);

    return {
        inner: innerPoints,
        outer: outerPoints,
    }
}

function getAllPoints(str) {
    return str.split(',(').map(item => item.replace(/[()]/g, '')).map(point => {
        const [x, y] = point.split(',');
        return {x, y};
    });
}

function renderPoints(points) {
    for (let i = 0; i < points.outer.length; i++) {
        const point = points.outer[i];
        const coords = coordInPixels(point.x, point.y);
        renderPoint(coords.x, coords.y, colors.outerPoint)
    }

    for (const point of points.inner) {
        const coords = coordInPixels(point.x, point.y);
        renderPoint(coords.x, coords.y, colors.innerPoint)
    }
}


function initialCanvas() {
    ctx.fillStyle = colors.darkGrey;
    ctx.fillRect(0, 0, canvas.width, canvas.height);
}

function calcMapSize(map) {
    const margin = 30;
    const boundingRect = calcMapEdgePoints(map.edges);

    const widthRatio = canvas.width / boundingRect.width;
    const heightRatio = canvas.height / boundingRect.height;
    let pixelSize;

    if (widthRatio < heightRatio) {
        pixelSize = (canvas.width - margin * 2) / boundingRect.width;
    } else {
        pixelSize = (canvas.height - margin * 2) / boundingRect.height;
    }

    const widthInPixels = pixelSize * boundingRect.width;
    const heightInPixels = pixelSize * boundingRect.height;

    return  {
        ...boundingRect,
        margin,
        pixelSize,
        widthInPixels,
        heightInPixels,
        left: (canvas.width - widthInPixels) / 2,
        top: (canvas.height - heightInPixels) / 2,
    }
}

function calcMapEdgePoints(points) {
    let minX, minY, maxX, maxY;

    for (let point of points) {
        point.x = + point.x;
        point.y = + point.y;
        if (!minX || point.x < minX) {
            minX = point.x;
        }
        if (!maxX || point.x > maxX) {
            maxX = point.x;
        }
        if (!minY || point.y < minY) {
            minY = point.y;
        }
        if (!maxY || point.y > maxY) {
            maxY = point.y;
        }
    }

    return {
        minX,
        minY,
        maxX,
        maxY,
        width: maxX - minX,
        height: maxY - minY,
    }
}

function renderMap(map) {
    ctx.fillStyle = colors.lightGrey;
    ctx.beginPath();
    const coords = coordInPixels(map.edges[0].x, map.edges[0].y);
    ctx.moveTo(coords.x, coords.y);

    for (const point of map.edges) {
        const coords = coordInPixels(point.x, point.y);
        ctx.lineTo(coords.x, coords.y);
    }

    ctx.fill();
}

function coordInPixels(x, y) {
    return {
        x: x * config.pixelSize + config.left,
        y: canvas.height - (y * config.pixelSize) - (config.top / 2),
    }
}

function renderPoint(x, y, color) {
    ctx.fillStyle = color;
    ctx.strokeStyle = 'black';
    ctx.beginPath();
    ctx.arc(x + config.pixelSize / 2, y - config.pixelSize / 2, config.pixelSize / 2, 0, Math.PI * 2);
    ctx.fill();
}