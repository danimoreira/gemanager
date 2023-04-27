'use strict';
const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const changed = require('gulp-changed');
const gutil = require('gulp-util');
//var ftp = require( 'vinyl-ftp' );
const sourcemaps = require('gulp-sourcemaps');
const errorHandler = require('gulp-error-handle');
const logError = function (err) {
    gutil.log(err);
    //this.emit('end');
};
const minify = require('gulp-minify');
const { watch } = require('gulp');
function upload() {
    const project_globs = ['./css/**', './woocommerce/**', './*'];
    watch(project_globs, function (cb) {
        var conn = ftp.create({
            host: 'ftp.lojadorafael.com.br',
            user: 'u768240244',
            password: 'lr@2018',
            parallel: 5,
            log: gutil.log
        });

        return gulp.src(project_globs, { base: '.', buffer: false })
            .pipe(errorHandler(logError))
            .pipe(conn.newerOrDifferentSize('/public_html/wp-content/themes/bs'))
            .pipe(conn.dest('/public_html/wp-content/themes/bs'));

    });
}

function watch_assets() {
    watch(['Content/sass/*.scss'], function (cb) {
        // body omitted
        gulp.src('Content/sass/*.scss')
            .pipe(errorHandler(logError))
            .pipe(changed('css'))
            .pipe(sass({ outputStyle: 'compressed' })
                .on('error', sass.logError))
            .pipe(sourcemaps.init())
            .pipe(sourcemaps.write('maps'))
            .pipe(gulp.dest('Content'));
        cb();
    });
}
function w(cb) {
    watch_assets();
    //upload();
    cb();
}
exports.w = w;
