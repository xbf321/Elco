(function () {
    tinymce.create('tinymce.plugins.xuimgPlugin', {
        bookMark: null,
        init: function (ed, url) {
            var This = this;
            This.editor = ed;
            This.url = url;
            ed.addCommand('mceXuimg', function () {
                //This.bookMark = ed.selection.getBookmark(); //Fix IE BUG 保存焦点
                ed.windowManager.open({
                    file: url + '/upload.htm?v='+(new Date()).getTime(),
                    width: 430,
                    height: 200,
                    inline: 1
                }, {
                    plugin_url: url
                });
            });
            ed.addButton('xuimg', {
                title: 'Upload local image',
                cmd: 'mceXuimg',
                image: url + '/img/image.gif'
            });
        },
        createControl: function (n, cm) {
            return null;
        }
    });
    tinymce.PluginManager.add('xuimg', tinymce.plugins.xuimgPlugin);
})();