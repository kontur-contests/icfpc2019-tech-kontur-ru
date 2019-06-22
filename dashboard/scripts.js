
// const rowsHtml = Object.keys(rows).map(i => `<tr><td>${i}</td>${algos.map(x => "<td " + (Object.values(rows[i]).every(z => z >= rows[i][x]) ? 'class="min"' : (!rows[i][x] ? 'class="no"' : "")) + ">" + (rows[i][x] || "") + "</td>").join("")}</tr>`).join("")


let algs = new Set();
const formattedData = mapData(dataFromServer);
const tenMinutes = 10 * 60 * 1000;
createTable();


function createTable() {
    const body = document.getElementsByTagName('body')[0];
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

    [...algs].forEach(item => {
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
        [...algs].forEach(item => {
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