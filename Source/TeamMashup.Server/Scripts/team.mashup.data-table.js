function TeamMashupDataTable(options) {

    var settings = $.extend({
        rowsSelectable: true,
        showEdit: true,
        showEditInNewWindow: true,
        showDelete: true,
        showMoreOptions: true,
        enableSearch: false,
        showDefaultSearchBar: false
    }, options);

    var header = $('#' + settings.tableId).find("thead");
    header.append("<tr></tr>");
    var headerRow = $('#' + settings.tableId).find("thead tr");

    var predefinedColumns = new Array();

    if (settings.rowsSelectable) {

        predefinedColumns.push({
            "mData": null,
            "sClass": "grid-field-20",
            "bSortable": false,
            "mRender": function (data, type, full) {
                return "<input type=\"checkbox\" class=\"row-checkbox\"/>";
            }
        });

        headerRow.append("<th><input type=\"checkbox\" id=\"selectAllRows\"/></th>");
    }

    if (settings.showEdit) {

        predefinedColumns.push({
            "mData": null,
            "sClass": "grid-field-20 grid-icon",
            "bSortable": false,
            "mRender": function (data, type, full) {
                return "<button type=\"button\" class=\"btn btn-xs btn-default btn-edit\"><i class=\"icon-pencil\" title=\"Edit\" /></button>";
            }
        });

        headerRow.append("<th></th>");
    }

    if (settings.showEditInNewWindow)
    {
        predefinedColumns.push({
            "mData": null,
            "sClass": "grid-field-20 grid-icon",
            "bSortable": false,
            "mRender": function (data, type, full) {
                return "<button type=\"button\" class=\"btn btn-xs btn-default btn-edit-new\"><i class=\"icon-edit\" title=\"Edit in new Tab/Window\" /></button>";
            }
        });

        headerRow.append("<th></th>");
    }

    if (settings.showDelete) {

        predefinedColumns.push({
            "mData": null,
            "sClass": "grid-field-20 grid-icon",
            "bSortable": false,
            "mRender": function (data, type, full) {
                return "<button type=\"button\" class=\"btn btn-xs btn-default btn-delete\"><i class=\"icon-remove\" title=\"Delete\" /></button>";
            }
        });

        headerRow.append("<th></th>");
    }

    if (settings.showMoreOptions) {

        predefinedColumns.push({
            "mData": null,
            "sClass": "grid-field-20 grid-icon",
            "bSortable": false,
            "mRender": function (data, type, full) {
                return "<button type=\"button\" class=\"btn btn-xs btn-default btn-options\"><i class=\"icon-cog\" title=\"Options\"></i></button>";
            }
        });

        headerRow.append("<th></th>");
    }

    var columns = predefinedColumns.concat(settings.columns);

    $(settings.columns).each(function () {
        headerRow.append("<th>" + $(this).attr("sTitle") + "</th>");
    });

    var tableOptions = {
        "bProcessing": true,
        "bServerSide": true,
        "bLengthChange": false,
        "bFilter": settings.enableSearch,
        "sPaginationType": "full_numbers",
        "sAjaxSource": settings.sourceUrl,
        "aoColumns": columns,
        "fnCreatedRow": function (nRow, aData, iDataIndex) {
            bindRowEvents($(nRow));
        },
        "fnDrawCallback": function (oSettings) {
            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
            }

            if (!settings.showDefaultSearchBar) {
                $("#" + settings.tableId + "_filter").hide();
            }

            if (settings.onDraw) {
                settings.onDraw();
            }
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
            
            if (settings.onRowClicked) {
                var rowId = $(nRow).attr("id");
                $(nRow).find("td").click({ rowId: rowId }, function (event) {
                    var index = event.currentTarget.cellIndex;
                    var column = columns[index];
                    var id = getOriginalId(event.data.rowId);
                    settings.onRowClicked(id, column);
                });
            }

            return nRow;
        },
    }

    var table;
    if (settings.reorderRows)
        table = $('#' + settings.tableId).dataTable(tableOptions).rowReordering();
    else
        table = $('#' + settings.tableId).dataTable(tableOptions);

    table.find("#selectAllRows").change(function () {
        var checked = $(this).is(":checked");

        if (checked) {
            selectAllRows();
        }
        else {
            deselectAllRows();
        }
    });

    this.deleteSelectedRows = function () {
        var ids = new Array();
        var selectedRows = getSelectedRows();
        selectedRows.each(function () {

            var id = getOriginalId($(this).attr("Id"));
            ids.push(id);

        });

        $.ajax({
            url: settings.deleteUrl,
            type: "POST",
            data: JSON.stringify({ ids: ids }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response.Success) {
                    table.fnDraw();
                    table.find("#selectAllRows").prop("checked", false);
                }
            }
        });
    }

    this.selectAllRows = selectAllRows;

    this.deselectAllRows = deselectAllRows;

    this.draw = function () {
        table.fnDraw();
    }

    this.search = function (searchTerm) {
        
        table.fnFilter(searchTerm);
    }

    this.getOriginalId = function (rowId) {
        return getOriginalId(rowId);
    }

    this.reload = function (url) {
        table.fnReloadAjax(url);
    }

    function bindRowEvents(row) {

        var id = getOriginalId(row.attr("Id"));

        row.find(".btn-edit").click(function () {
            settings.onItemEdit(id, false);
        });

        row.find(".btn-edit-new").click(function () {
            settings.onItemEdit(id, true);
        });

        row.find(".btn-delete").click(function () {
            settings.onItemDelete(id);
        });

        row.find(".row-checkbox").click(function () {
            row.addClass("selected");
        });
    }

    function getRows() {
        return $("#" + settings.tableId + " tbody tr");
    }

    function getSelectedRows() {
        return $("#" + settings.tableId + " tbody tr.selected");
    }

    function getOriginalId(rowId) {
        return rowId.split("_")[1];
    }
    
    function selectAllRows() {
        var rows = getRows();
        rows.each(function () {
            $(this).addClass("selected");
            $(this).find(".row-checkbox").prop("checked", true);
        });
    }

    function deselectAllRows() {
        var selectedRows = getSelectedRows();
        selectedRows.each(function () {
            $(this).removeClass("selected");
            $(this).find(".row-checkbox").prop("checked", false);
        });
    }

    return this;
}