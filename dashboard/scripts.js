
const body = document.getElementsByTagName('body')[0];

let hiddenAlgs = getHiddenColumns();
let algs = new Set();
const formattedData = mapData(dataFromServer);
const tenMinutes = 10 * 60 * 1000;
renderShowAllButton();
createTable();


function getHiddenColumns() {
    return JSON.parse(localStorage.getItem('hiddenColumn')) || [];
}

function renderShowAllButton() {
    if (!hiddenAlgs || !hiddenAlgs.length || document.getElementsByClassName('show-all').length) {
        return;
    }


    const button = document.createElement('button');
    button.textContent = 'Показать все алгоритмы';
    button.classList.add('show-all');
    button.addEventListener('click', showAll);

    body.appendChild(button);

}

function createTable() {
    const table = document.createElement('table');
    table.classList.add('table');


    const tableHeader = document.createElement('thead');
    const tableHeaderRow = document.createElement('tr');

    const indexTh = document.createElement('th');
    tableHeaderRow.appendChild(indexTh);

    const bestTh = document.createElement('th');
    bestTh.classList.add('min');
    bestTh.innerText = 'best score';
    tableHeaderRow.appendChild(bestTh);

    const algsOrder = [...algs].sort().filter(alg => !hiddenAlgs.includes(alg));

    algsOrder.forEach(item => {
        const th = document.createElement('th');
        th.textContent = `${item}`;

        tableHeaderRow.appendChild(th);
    });

    const tableBody = document.createElement('tbody');

    Object.keys(formattedData).sort((a, b) => a - b).forEach((num) => {
        const tr = document.createElement('tr');
        const index = document.createElement('td');
        index.textContent = num;

        const bestTd = document.createElement('td');
        bestTd.classList.add('min');

        tr.appendChild(index);
        tr.appendChild(bestTd);


        const row = formattedData[num];
        algsOrder.forEach(item => {
            const td = document.createElement('td');
            td.textContent = (row[item] && row[item].result) || '';


            if (!row[item] || !row[item].result) {
                td.classList.add('no');
            } else if (Object.values(row).every(el => el.result >= row[item].result)) {
                td.classList.add('min');
                bestTd.innerHTML = `<b>${row[item].result}</b><br>${item}`;
            }

            const now = Date.now();

            if (row[item] && now - row[item].timestamp * 1000 < tenMinutes) {
                td.classList.add('recent');
            }

            tr.appendChild(td);
        });

        tableBody.appendChild(tr);
    });

    tableHeader.appendChild(tableHeaderRow);
    table.appendChild(tableHeader);
    table.appendChild(tableBody);
    body.appendChild(table);

    addListeners();
}

function mapData() {
    return dataFromServer.reduce((acc, item) => {
        const [id, alg, version] = item._id.split('_');
        const algName = `${alg} v${version}`;


        algs.add(algName);

        if (!acc[id]) {
            acc[id] = {}
        }

        acc[id][algName] = {
            result: item.time,
            timestamp: item.timestamp,
        };

        return acc;
    }, {})
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
    createTable();
}

function showAll() {
    hiddenAlgs = [];

    localStorage.setItem("hiddenColumn", JSON.stringify(hiddenAlgs));

    deleteTable();
    createTable();


    const button = document.getElementsByClassName('show-all')[0];

    body.removeChild(button);
}
