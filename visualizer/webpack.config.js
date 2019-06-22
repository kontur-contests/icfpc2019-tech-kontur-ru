const path = require('path');

module.exports = {
    entry: {
        solutions: './files.js',
        puzzles: './files-puzzles.js'},
    output: {
        filename: '[name].js',
        publicPath: 'dist',
    },
    module: {
        rules: [
            {
                test: /\.(desc|sol|cond)$/i,
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