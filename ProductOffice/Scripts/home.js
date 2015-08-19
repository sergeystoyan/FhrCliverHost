function show_message(content, title) {
    if (!title)
        title = '&nbsp;';
    var html = '<div title="' + title + '"><p>' + content + '</p></div>';
    var e = $(html);
    $("body").append(e);

    e.on('dialogclose', function (event) {
        e.remove();
    });

    e.dialog({
        resizable: true,
        height: 'auto',
        width: 'auto',
        modal: true,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        },
        show: {
            effect: "clip",
            duration: 400
        },
        hide: {
            effect: "fade",
            duration: 400
        }
    });

    set_modal_window(e);
    return e;
}

function show_error(content, title) {
    if (!title)
        title = "ERROR";
    var html = '<div title="' + title + '" style="color:#f00;"><p><span class="ui-icon-alert" style="float:left; margin:0 7px 20px 0;""></span>' + content + '</p></div>';
    var e = $(html);
    $("body").append(e);

    e.on('dialogclose', function (event) {
        e.remove();
    });

    e.dialog({
        resizable: true,
        height: 'auto',
        width: 'auto',
        modal: true,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        },
        show: {
            effect: "highlight",
            duration: 400
        },
        hide: {
            //effect: "blind",
            effect: "fade",
            duration: 400
        }
    });

    set_modal_window(e);
    return e;
}

function set_modal_window(e) {
    //var p = $(e).closest('.ui-dialog-content');
    ////console.log($(e).html());
    //if (p) {
    //    //console.log(c.parent().parent().html());
    //    //var p = c.parent();
    //    var h = p[0].scrollHeight - p.height();
    //    //console.log(c[0].scrollHeight + ':' + c.innerHeight() + ':' + c.outerHeight() + ':' + c.outerHeight(true) + ':' + c.height());
    //    console.log(p[0].scrollHeight + ':' + p.innerHeight() + ':' + p.outerHeight() + ':' + p.outerHeight(true) + ':' + p.height());
    //    var h2 = $(window).height() - $(e).parent().height();
    //    if (h > h2)
    //        h = h2;
    //    $(e).height($(e).height() + h);

    //    var w = p.scrollWidth - p.width();
    //    var w2 = $(window).width() - $(e).parent().width();
    //    if (w > w2)
    //        w = w2;
    //    $(e).width($(e).width() + w);

    //    console.log(h + '-' + w);
    //}
    //else {
    var h = $(e).parent().height() - $(window).height();
    if (h > 0)
        $(e).height($(e).height() - h - 10);
    var w = $(e).parent().width() - $(window).width();
    if (w > 0)
        $(e).width($(e).width() - w - 10);
    else {
        if (w < -50)
            w = -50;
        $(e).width($(e).width() - w - 10);
    }
    //} 
    e.dialog({
        "position": {
            my: "center", at: "center", of: window, collision: 'fit',
        },
    });
        //$(e).parent().offset({ 'top': ($(window).height() - $(e).parent().height()) / 2, 'left': ($(window).width() - $(e).parent().width()) / 2 });
        //$(e).parent().offset({ 'top': ($(window).height() - $(e).parent().height()) / 2, 'left': ($(window).width() - $(e).parent().width()) / 2 });
    //console.log(($(window).width() + ' - ' + $(e).parent().width()) + ':' + ($(window).width() - $(e).parent().width()) / 2);
}

function show_modal_box(dialog_id, title, buttons) {
    if (!dialog_id)
        dialog_id = '';
    if (!title)
        title = '&nbsp;';
    var html = '<div class="' + dialog_id + '" title="' + title + '"><div class="_loading" style="height:100%;width:100%;position:absolute;z-index:10;"><img src="/Images/ajax-loader.gif" style="display:block;margin:auto;position:relative;top:50%;transform:translateY(-50%);"/></div><div class="_content"></div></div>';
    var e = $(html);
    $("body").append(e);

    e.on('dialogclose', function (event) {
        e.remove();
    });
    
    if (!buttons) {
        buttons["Cancel"] = null;
    }
    $.each(buttons, function (name, value) {
        if(!value)
            buttons[name] = function () {
            e.remove();
        }
    });

    e.dialog({
        resizable: true,
        height: 'auto',
        width: 'auto',
        modal: true,
        buttons: buttons,
        show: {
            effect: "fade",
            duration: 400
        },
        hide: {
            effect: "fade",
            duration: 400
        }
    });
    
    e.processing = function (show) {
        if (show || show == undefined) {
            e.find("._loading").show();
            e.find("._content").css('visibility', 'hidden');
        }
        else {
            e.find("._loading").hide();
            e.find("._content").css('visibility', 'visible');
        }
    }
    
    e.title = function (html) {
        if (html == undefined)
            return e.parent().find(".ui-dialog-title").html();
        e.parent().find(".ui-dialog-title").html(html);
    }

    e.content = function (html) {
        if (html == undefined)
            return e.find("._content").html();
        e.find("._content").html(html);
    }

    e.getContentByAjax = function (ajax_config, on_success) {
        if (ajax_config["type"] == undefined)
            ajax_config["type"] = "POST";
        ajax_config["success"] = function (response) {
            e.processing(false);
            on_success(response);
            set_modal_window(e);
        };
        ajax_config["error"] = function (xhr, error) {
            e.processing(false);
            //e.parent().find(".ui-dialog-title").html("Server Error");
            //e.content("<span style='color:red;font-weight:bold;'>" + error + "</span><hr>" + xhr.responseText);
            //e.content(e.content() + xhr.responseText);
            //set_modal_window(e);
            show_error(xhr.responseText, error);
        };
        e.processing();
        $.ajax(ajax_config);
    }

    return e;
}

function show_table_row_editor(content_url, ok_button_text, on_success) {
    var e;
    
    var buttons = {};
    if (on_success) {
        buttons[ok_button_text] = function () {
            if (!e.find("form").valid())
                return;

            e.processing();

            $.ajax({
                type: e.find("form").attr('method'),
                url: e.find("form").attr('action'),
                data: e.find("form").serialize(),
                success: function (data) {
                    if (!data || data.redirect) {
                        e.remove();
                        on_success();
                        return;
                    }
                    e.processing(false);
                    e.content(data);
                },
                error: function (xhr, error) {
                    e.processing(false);
                    show_error(xhr.responseText);
                }
            });
        };
        buttons["Cancel"] = function () {
            e.remove();
        }
    }
    else {
        buttons[ok_button_text] = function () {
            e.remove();
        }
    }
    
    e = show_modal_box(null, null, buttons);

    e.getContentByAjax(
        {
            type: "GET",
            url: content_url,
        },
        function (data) {
                var title;
                var r = /\<h2\s*[^>]*\>([^]*)\<\/h2\s*[^>]*\>/mi;
                var m = r.exec(data);
                if (m) {
                    title = m[0];
                    data = data.replace(r, "");
                }
                e.title(title);

                e.content(data);
                e.find("form").find("input[type='submit']").parent().hide();

                e.find("form").validate();//does not work properly
                //$.getScript("/Scripts/jquery.validate.js");
                //$.getScript("/Scripts/jquery.validate.unobtrusive.js");
            //e.find("form").valid();
        }
    );

    return e;
}

function init_table(server_side, id_column_id, new_button_text, details_button_text, edit_button_text, delete_button_text, path, invisible_column_ids) {
    var definition = {
        "scrollX": true,
        "processing": true,
        "language": {
            "processing": '<img src="/Images/ajax-loader.gif" style="z-index:1;position:relative"/>'
        },
        "paging": true,
        //"columnDefs": [
        //    { "visible": false, "targets": 0 },
        //],
        //"columns": [
        //  { "visible": false },
        //  null,
        //  null,
        //],
        //"stateSave": true,
        "rowCallback": function (row, data, index) {
            h = $(row).html().replace(/(\d{4}\-\d{2}\-\d{2})T(\d{2}\:\d{2}:\d{2})(\.\d+)?/i, "$1 $2");
            h = h.replace(/(<a\s.*?<\/a\s*>|https?\:\/\/[^\s<>\'\"]*)/i, function (m) {
                if (m[0] == "<") {
                    return m;
                }
                return "<a href=\"" + m + "\">" + m + "</a>";
            });
            $(row).html(h);
        }
    };
    if (server_side) {
        definition["serverSide"] = true;
        definition["ajax"] = {
            "url": path + "/TableJson",
            "type": 'POST',
        };
    }
    if (invisible_column_ids) {
        definition["columnDefs"] = Array();
        for (var i = invisible_column_ids.length - 1; i >= 0; i--)
            definition["columnDefs"].push({ "visible": false, "targets": invisible_column_ids[i] });
    }

    var table = $("table:last").dataTable(definition);
    
    //table.columns[id_column_id].visible(show_id_column);

    if (server_side) {
        var search_box = table.parent().find(".dataTables_filter").find("input");
        //search_box.keyup(function () {
        search_box.on('keyup', function (event) {
            if (event.keyCode == 13) {
                table.api().search(search_box.val()).draw();
            }
        });
    }

    var left_menu_html = '<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>';
    var left_menu = $(left_menu_html);
    $("body").append(left_menu);
    var right_menu_html = '<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>';
    var right_menu = $(right_menu_html);
    $("body").append(right_menu);
    if (new_button_text) {
        var buttons = $("<p></p>");
        table.parents(".dataTables_wrapper").before(buttons);
        var b = $('<a href="#" class="button">' + new_button_text + '</a>');
        buttons.append(b);

        b.click(function () {
            table.modalBox = show_table_row_editor(path + "/Create", "Create", function () {
                if (server_side)
                    table.api().draw();
                else
                    location.reload();
            });
        });
    }
    if (details_button_text) {
        var b = $('<a href="#" class="button">' + details_button_text + '</a>');
        right_menu.append(b);

        b.click(function () {
            if (!table.$('tr.selected').is("tr")) {
                show_message("No row selected!", "Warning");
                return false;
            }
            var id = table.fnGetData(table.$('tr.selected'))[id_column_id];

            table.modalBox = show_table_row_editor(path + "/Details?Id=" + id, "OK");
        });
    }
    if (edit_button_text) {
        var b = $('<a href="#" class="button">' + edit_button_text + '</a>');
        right_menu.append(b);

        b.click(function () {
            if (!table.$('tr.selected').is("tr")) {
                show_message("No row selected!", "Warning");
                return false;
            }
            var id = table.fnGetData(table.$('tr.selected'))[id_column_id];

            table.modalBox = show_table_row_editor(path + "/Edit?Id=" + id, "Save", function () {
                if (server_side)
                    table.api().draw();
                else
                    location.reload();
            });
        });
    }
    if (delete_button_text) {
        //var b = $('<a href="#">' + delete_button_text + '</a>');
        var b = $('<a href="#" class="button" style="color:#f00;">' + 'X' + '</a>');
        left_menu.append(b);

        b.click(function () {
            if (!table.$('tr.selected').is("tr")) {
                show_message("No row selected!", "Warning");
                return false;
            }
            var id = table.fnGetData(table.$('tr.selected'))[id_column_id];

            table.modalBox = show_table_row_editor(path + "/Delete?Id=" + id, delete_button_text, function () {
                if (server_side)
                    table.api().draw();
                else
                    location.reload();
            });
        });
    }
    if (!left_menu.html())
        left_menu = null;
    if (!right_menu.html())
        right_menu = null;
    if (left_menu || right_menu) {        
        table.find('tbody').on('click', 'tr', function () {
            var row = $(this);
            if (row.hasClass('selected')) {
                row.removeClass('selected');
            }
            else {
                table.$('tr.selected').removeClass('selected');
                row.addClass('selected');
            }
            if (row.hasClass('selected')) {
                var t = row.offset().top;
                var r = table.parents(".dataTables_wrapper");
                if (left_menu) {
                    left_menu.css('visibility', 'visible');
                    left_menu.offset({'top': t, 'left': r.offset().left - left_menu.outerWidth()});
                    left_menu.css("padding-top", row.find('td:first').css("padding-top"));
                    left_menu.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    left_menu.innerHeight(row.innerHeight());
                }
                if (right_menu) {
                    right_menu.css('visibility', 'visible'); 
                    right_menu.offset({ 'top': t, 'left': r.offset().left + r.outerWidth(true)});
                    right_menu.css("padding-top", row.find('td:first').css("padding-top"));
                    right_menu.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    right_menu.innerHeight(row.innerHeight());
                }
            }
            else {
                if (left_menu)
                    left_menu.css('visibility', 'hidden');
                if (right_menu)
                    right_menu.css('visibility', 'hidden');
            }
        });
        table.on('draw.dt', function () {
            if (left_menu)
                left_menu.css('visibility', 'hidden');
            if (right_menu)
                right_menu.css('visibility', 'hidden');
        });
    }

    return table;
}
