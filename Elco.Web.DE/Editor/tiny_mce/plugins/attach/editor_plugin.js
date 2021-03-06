﻿(function () {
    tinymce.create('tinymce.plugins.attachPlugin', {
        bookMark: null,
        init: function (ed, url) {
            var This = this;
            This.editor = ed;
            This.url = url;
            ed.addCommand('mceAttach', function () {
                //This.bookMark = ed.selection.getBookmark(); //Fix IE BUG 保存焦点
                ed.windowManager.open({
                    file: url + '/attach.htm?v=201207171546',
                    width: 430,
                    height: 250,
                    inline: 1,
                    title: 'Insert Attachment'
                }, {
                    plugin_url: url
                });
            });
            // Register buttons
            ed.addButton('attach', {
                title: 'Insert Attachment',
                cmd: 'mceAttach',
                image: url + '/img/zip.gif'
            });
        },
        createControl: function (n, cm) {
            return null;
        }
    });
    tinymce.PluginManager.add('attach', tinymce.plugins.attachPlugin);
})();