const MongoClient = require('mongodb').MongoClient;
const fs = require('fs')

const dbHost = "mongodb://icfpc19-mongo1:27017";
const dbName = "icfpc";
const metaCollectionName = "solution_metas";

var pipeline = [
    {
        $group: {
            "_id": {
                $concat: [
                    { $toString: "$ProblemId" },
                    "_",
                    "$AlgorithmId",
                    "_",
                    { $toString: "$AlgorithmVersion" }
                ]
            },
            "time": {
                $min: "$OurTime"
            }
        }
    }
]

MongoClient
    .connect(dbHost, { useNewUrlParser: true })
    .then(c => c.db(dbName))
    .then(db => db.collection(metaCollectionName).aggregate(pipeline).toArray())
    .then(data => {
        var rows = {}
        var algos = []

        data.forEach(row => {
            var info = row._id.split('_')

            var id = info[0]
            var algo = `${info[1]} v${info[2]}`

            if (algos.indexOf(algo) === -1) {
                algos.push(algo)
            }

            if (rows[id] === undefined) {
                rows[id] = {}
            }

            rows[id][algo] = row.time
        })

        algos.sort();

        // console.log(rows)

        const styles = `<style>.min {background-color:#cfc} .no {background-color:#ccc}</style>`
        const meta = `<META HTTP-EQUIV="REFRESH" CONTENT="10;URL=/">`
        const headerHtml = `<th>${algos.map(x => "<th>" + x + "</th>").join("")}</tr>`
        const rowsHtml = Object.keys(rows).map(i => `<tr><td>${i}</td>${algos.map(x => "<td " + (Object.values(rows[i]).every(z => z >= rows[i][x]) ? 'class="min"' : (!rows[i][x] ? 'class="no"' : "")) + ">" + (rows[i][x] || "") + "</td>").join("")}</tr>`).join("")
        const tableHtml = `${styles}${meta}<table border='1' cellspacing='0' cellpadding='5'>${headerHtml}${rowsHtml}</table>`

        fs.writeFileSync("dashboard.html", tableHtml)
    })

setInterval(() => process.exit(0), 5000)
