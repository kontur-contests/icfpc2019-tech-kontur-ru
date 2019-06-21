const path = require('path');

module.exports = {
    entry: './files.js',
    output: {
        filename: 'bundle.js',
    },
    module: {
        rules: [
            {
                test: /\.(desc|sol)$/i,
                use: 'raw-loader',
            },
        ],
    },
    devServer: {
        contentBase: path.join(__dirname, '/'),
        compress: true,
        port: 9000
    },
};