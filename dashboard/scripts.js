const TEN_MINUTES = 10 * 60 * 1000;
const body = document.getElementsByTagName('body')[0];

let hiddenAlgs = getHiddenColumns();
let showTable = getState();
const formattedData = mapData(dataFromServer);
const formattedBlockchainData = mapData(blockchainDataForScript);
const formattedDataProgress = mapData(progressDataForScript);
renderState();


function getHiddenColumns() {
    return JSON.parse(localStorage.getItem('hiddenColumn')) || [];
}

function getState() {
    return localStorage.getItem('showTable') || 'base';
}

function mapData(rawData) {
    const algs = new Set();
    const tasks = new Set();
    const formattedData = rawData.reduce((acc, item) => {
        const [id, alg, version] = item._id.split('_');
        const algName = `${alg} v${version}`;

        tasks.add(parseInt(id));
        algs.add(algName);

        if (!acc[id]) {
            acc[id] = {}
        }

        acc[id][algName] = {
            ...item,
        };

        return acc;
    }, {});

    return {
        algs: [...algs].sort(),
        tasks: [...tasks].sort((a, b) => a - b),
        data: formattedData,
    }
}

function renderState() {
    renderControls();
    let data;
    let needProgress = false;


    if (showTable === 'base') {
        data = formattedData;
        needProgress = true;
    } else {
        data = formattedBlockchainData;
    }
    const bests = calcBests(data.data);

    renderTable(data.data, data.algs, data.tasks, bests, needProgress);

}

function renderControls() {
    let wrapper;

    if (!document.getElementsByClassName('controls').length) {
        wrapper = document.createElement('div');
        wrapper.classList.add('controls');
        body.appendChild(wrapper);
    } else {
        wrapper = document.getElementsByClassName('controls')[0];
    }

    renderToggle(wrapper);

    if (showTable === 'base') {
        renderShowAllButton();
    }


}

function renderShowAllButton() {
    if (!hiddenAlgs || !hiddenAlgs.length ||
        document.getElementsByClassName('show-all').length ||
        showTable !== 'base') {
        return;
    }

    const container = document.getElementsByClassName('controls')[0];
    const button = document.createElement('button');
    button.textContent = 'Показать все алгоритмы';
    button.classList.add('show-all');
    button.addEventListener('click', showAll);

    container.appendChild(button);

}

function renderToggle(container) {
    if (document.getElementsByClassName('toggle').length) {
        return;
    }

    const toggle = document.createElement('label');
    toggle.classList.add('toggle');

    const checkbox = document.createElement('input');
    checkbox.classList.add('toggle-input');
    checkbox.type = 'checkbox';
    checkbox.checked = showTable !== 'base';
    checkbox.addEventListener('change', stateUpdate);

    const rightText = document.createElement('span');
    rightText.classList.add('toggle-text');
    rightText.textContent = 'Показывать паззлы';

    toggle.appendChild(checkbox);
    toggle.appendChild(rightText);
    container.appendChild(toggle);

}

function stateUpdate(e) {
    const value = e.target.checked;
    showTable = value ? 'puzzles' : 'base';

    localStorage.setItem("showTable", showTable);

    deleteShowAllButton();
    deleteTable();
    renderState();
}

function calcBests(data) {
    const bests = {};
    for (const taskNum of Object.keys(data)) {
        const taskTries = data[taskNum];
        for (const algName of Object.keys(taskTries)) {
            const algRes = taskTries[algName];
            algRes.algName = algName;
            if (!bests[taskNum] || bests[taskNum].time > algRes.time) {
                bests[taskNum] = algRes;
            }
        }
    }

    return bests;
}

function renderTable(data, algs, tasks, bests, needProgress) {
    const table = document.createElement('table');
    table.classList.add('table');

    const algsOrder = algs.filter(alg => !hiddenAlgs.includes(alg));
    const tableHeader = createTableHeader(algsOrder);

    const tableBody = createTableBody(data, algsOrder, tasks, bests, needProgress);

    table.appendChild(tableHeader);
    table.appendChild(tableBody);
    body.appendChild(table);


    if (showTable === 'base') {
        addListeners();
    }
}

function createTableHeader(algs) {
    const tableHeader = document.createElement('thead');
    const tableHeaderRow = document.createElement('tr');

    const indexTh = document.createElement('th');
    tableHeaderRow.appendChild(indexTh);

    const bestTh = document.createElement('th');
    bestTh.innerText = 'best score';
    tableHeaderRow.appendChild(bestTh);


    algs.forEach(item => {
        const th = document.createElement('th');
        th.textContent = `${item}`;

        tableHeaderRow.appendChild(th);
    });

    tableHeader.appendChild(tableHeaderRow);

    return tableHeader;
}

function createTableBody(data, algs, tasks, bests, needProgress) {
    const tableBody = document.createElement('tbody');
    const now = Date.now();

    tasks.sort((a, b) => showTable === 'base' ? a - b : b - a).forEach(task => {
        const tr = document.createElement('tr');
        const indexTd = document.createElement('td');
        indexTd.textContent = task;

        const bestTd = document.createElement('td');
        bestTd.classList.add('min');
        bestTd.innerHTML = `<b>${bests[task].time}</b><br>${bests[task].algName}`;

        tr.appendChild(indexTd);
        tr.appendChild(bestTd);

        tableBody.appendChild(tr);

        const rowData = data[task];

        algs.forEach(alg => {
            const td = document.createElement('td');
            td.textContent = (rowData[alg] && rowData[alg].time) || '';

            if (!rowData[alg] || !rowData[alg].time) {
                td.classList.add('no');
            } else if (bests[task].algName === alg) {
                td.classList.add('min');
            }


            if (rowData[alg] && now - rowData[alg].timestamp * 1000 < TEN_MINUTES) {
                td.classList.add('recent');
                td.setAttribute('title', `Посчитан недавно`)
            }

            if (needProgress && formattedDataProgress.data[task] && formattedDataProgress.data[task][alg]) {
                if (!rowData[alg] || !rowData[alg].time) {
                    td.classList.add('in-progress');
                    td.textContent = '⏳';
                    td.setAttribute('title', `Алгоритм выполняется на ${formattedDataProgress.data[task][alg].hostName}`)
                }
            }

            tr.appendChild(td);
        });
    });

    return tableBody;
}

function addListeners() {
    const ths = [...document.getElementsByTagName('th')];

    for (let i = 0; i < ths.length; i++) {
        ths[i].addEventListener('click', hideColumn)
    }

}

function deleteTable() {
    const table = document.getElementsByClassName('table')[0];

    body.removeChild(table);
}

function hideColumn(e) {
    const algId = e.target.innerText;

    if (!hiddenAlgs) {
        hiddenAlgs = [];
    }

    hiddenAlgs.push(algId);

    localStorage.setItem("hiddenColumn", JSON.stringify(hiddenAlgs));

    deleteTable();
    renderShowAllButton();
    renderState();
}

function showAll() {
    hiddenAlgs = [];

    localStorage.setItem("hiddenColumn", JSON.stringify(hiddenAlgs));

    deleteTable();
    renderState();

    deleteShowAllButton();

}

function deleteShowAllButton() {
    const button = document.getElementsByClassName('show-all')[0];

    if (button) {
        button.parentNode.removeChild(button);
    }
}
