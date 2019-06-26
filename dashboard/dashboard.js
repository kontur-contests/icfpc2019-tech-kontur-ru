const MongoClient = require('mongodb').MongoClient;
const fs = require('fs');

const dbHost = "mongodb://icfpc19-mongo1:27017";
const dbName = "icfpc";
const metaCollectionName = "solution_metas";
const inprogressCollectionName = "solution_inprogress";
const blockchainCollectionName = "block_solution_metas";
const submissionCollectionName = "submission_summary";

const pipeline = [
    {
        $group: {
            "_id": {
                $concat: [
                    { $toString: "$ProblemId" },
                    "_",
                    "$AlgorithmId",
                    "_",
                    { $toString: "$AlgorithmVersion" },
                    "_",
                    { $toString: "$MoneySpent" }
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

const puzzlesPipeline = [
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

const inProgressPipeline = [
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
            "hostName": {
                $max: "$HostName"
            },
            "startedAt": {
                $max: "$StartedAt"
            }
        }

    }
];

const submissionPipeline = [
    {
        $group: {
            "_id": {
                $toString: "$ProblemId" ,
            },
            "moneySpent": {
                $max: "$MoneySpent"
            },
            "time": {
                $max: "$OurTime"
            }
        }

    }
];


const algsRequest = MongoClient
    .connect(dbHost, { useNewUrlParser: true })
    .then(c => c.db(dbName))
    .then(db => db.collection(metaCollectionName).aggregate(pipeline).toArray());

const blockchainRequest = MongoClient
    .connect(dbHost, { useNewUrlParser: true })
    .then(c => c.db(dbName))
    .then(db => db.collection(blockchainCollectionName).aggregate(puzzlesPipeline).toArray());

const inProgressRequest = MongoClient
    .connect(dbHost, { useNewUrlParser: true })
    .then(c => c.db(dbName))
    .then(db => db.collection(inprogressCollectionName).aggregate(inProgressPipeline).toArray());

const submissionRequest = MongoClient
    .connect(dbHost, { useNewUrlParser: true })
    .then(c => c.db(dbName))
    .then(db => db.collection(submissionCollectionName).aggregate(submissionPipeline).toArray());

Promise.all([algsRequest, inProgressRequest, blockchainRequest, submissionRequest]).then(([data, inProg, blockchain, submission]) => {
    const styles = '<link rel="stylesheet" href="styles.css">';
    const metaGeneral = '<meta charset="utf-8"><title>ICFPC 2019. Дашборд</title>';
    const meta = ''; //`<META HTTP-EQUIV="REFRESH" CONTENT="60;URL=/">`;
    const dataForScript = `<script type="text/javascript">const dataFromServer = ${JSON.stringify(data)}</script>`;
    const progressDataForScript = `<script type="text/javascript">const progressDataForScript = ${JSON.stringify(inProg)}</script>`;
    const blockchainDataForScript = `<script type="text/javascript">const blockchainDataForScript = ${JSON.stringify(blockchain)}</script>`;
    const submissionForScript = `<script type="text/javascript">const submissionForScript = ${JSON.stringify(submission)}</script>`;
    const scripts = '<script type="text/javascript" src="scripts.js"></script>';
    const tableHtml = `<head>${styles}${metaGeneral}${meta}</head><body>${dataForScript}${progressDataForScript}${blockchainDataForScript}${submissionForScript}${scripts}</body>`;

    fs.writeFileSync("dashboard.html", tableHtml);

} );



setInterval(() => process.exit(0), 5000);
