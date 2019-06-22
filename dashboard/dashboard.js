const MongoClient = require('mongodb').MongoClient;
const fs = require('fs');

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
            },
            "timestamp": {
                $max: "$SavedAt"
            }
        }
    }
];

MongoClient
    .connect(dbHost, { useNewUrlParser: true })
    .then(c => c.db(dbName))
    .then(db => db.collection(metaCollectionName).aggregate(pipeline).toArray())
    .then(data => {
        const styles = '<link rel="stylesheet" href="styles.css">';
        const metaGeneral = '<meta charset="utf-8"><title>ICFPC 2019. Дашборд</title>';
        const meta = `<META HTTP-EQUIV="REFRESH" CONTENT="10;URL=/">`;
        const dataForScript = `<script type="text/javascript">const dataFromServer = ${JSON.stringify(data)}</script>`;
        const scripts = '<script type="text/javascript" src="scripts.js"></script>';
        const tableHtml = `<head>${styles}${metaGeneral}${meta}</head><body>${dataForScript}${scripts}</body>`;

        fs.writeFileSync("dashboard.html", tableHtml)
    });

setInterval(() => process.exit(0), 5000);
