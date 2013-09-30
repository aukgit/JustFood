/// <reference path="jquery-2.0.3.js" />
/// <reference path="jquery-2.0.3.min.map" />
/// <reference path="jquery-2.0.3-vsdoc.js" />
/// <reference path="_references.js" />
/// <reference path="jquery.mobile-1.3.2.js" />

/*
 * Written by   : Alim Ul Karim
 * Email        : mailto:auk.junk@live.com 
 * Blog         : http://auk-port.blogspot.com/
 * Portfolio    : http://auk-port.zxq.net/
 * */

$(document).on('pageinit', function () {
    $('[data-role="page"]').trigger('create');

    var JqueryWaitFunction = function (wait) {
        // create a deferred object
        var r = $.Deferred();

        // do whatever you want (e.g. ajax/animations other asyc tasks)

        setTimeout(function () {
            // and call `resolve` on the deferred object, once you're done
            r.resolve();
        }, wait);

        // return the deferred object
        return r;
    };

    // save,remove button click
    // use jquery on when dynamically binding events to new objects
    $(".Inventory-Out-Config-Page").on('click', " a.clickedButtonOnInventoryOut", function (e) {
        e.preventDefault();
        var $this = $(this),
			inventoryId = $this.attr("data-inventory-id"),
			configId = $this.attr("data-config-id"),
			$form = $("form[data-inventory-id='" + inventoryId + "'][data-config-id='" + configId + "']"),
			actionLink = $form.attr("action"),
			task = "Saving",
            isRemoving = false,
            isAdding = false;

        // checking if we got 2 forms , basic structure one with.
        // if so then change the selector
        if ($form.length > 1) {
            // if so then change the selector
            // look for the item in the addObjects div.
            $form = $("div.addObjects-here form[data-inventory-id='" + inventoryId + "'][data-config-id='" + configId + "']")
        }
        //Checking if remove button.
        if ($this.hasClass("remove")) {
            actionLink = "/Admin/InventoryConfig/Remove";
            task = "Removing";
            if (!confirm('Do want to remove this item?')) {
                return; //break out of the function early
            }
            isRemoving = true;
        }
        if (configId === -1) {
            isAdding = true;
        }

        //        console.log($this);
        //        console.log($form);
        //       console.log($form.serialize());


        $.ajax({
            dataType: "json",
            data: $form.serialize(),
            type: $form.attr("method"),
            url: actionLink,

            beforeSend: function () {
                var $this = $(this),
							theme = "j" || $.mobile.loader.prototype.options.theme,
							msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
							textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
							textonly = !!$this.jqmData("textonly");
                html = $this.jqmData("html") || "";
                $.mobile.loading("show", {
                    text: task,
                    textVisible: true,
                    theme: theme,
                    textonly: textonly,
                    html: html
                });
            },

            success: function (data) {
                if (data.Status) {
                    // show the loading progressor
                    ShowSuccessLoading();
                    if (isAdding) {
                        $form.find("input[name='InventoryOutConfigID']").val(data.ConfigId);
                        $form.attr("data-config-id", data.ConfigId);
                    }
                    if (isRemoving) {
                        $form.fadeOut(500, function () {
                            $form.remove();                        
                        });
                    }
                    HideLoading(300);

                } else {
                    // failed in the transaction process
                    ShowErrorLoadingProcess(data);
                    HideLoading(1500);

                }
            },
            error: function (data) {
                ShowErrorLoadingProcess(data);
                HideLoading(600);
            }
        });

    });

    var ShowSuccessLoading = function () {
        var $this = $(this),
					theme = "j" || $.mobile.loader.prototype.options.theme,
					msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
					textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
					textonly = !!$this.jqmData("textonly");
        html = $this.jqmData("html") || "";
        $.mobile.loading("show", {
            text: "Done",
            textVisible: true,
            theme: theme,
            textonly: textonly,
            html: html
        });
    };

    var HideLoading = function (duration) {
        var hideLoading = function () {
            $.mobile.loading("hide");
        };
        JqueryWaitFunction(duration).done(hideLoading);
    };

    var ShowErrorLoadingProcess = function (data) {
        var $this = $(this),
					theme = "e" || $.mobile.loader.prototype.options.theme,
					msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
					textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
					textonly = !!$this.jqmData("textonly");
        html = $this.jqmData("html") || "";
        $.mobile.loading("show", {
            text: "Failed : " + data.Message,
            textVisible: true,
            theme: theme,
            textonly: textonly,
            html: html
        });

    };

    //add config button
    $(".Inventory-Out-Config-Page a.AddConfig").click(function (e) {
        e.preventDefault();
        var $this = $(this),
			inventoryId = $this.attr("data-inventory-id"),
			configId = -1,
			$divStructure = $("div.structure"),
			$divStartingPoint = $(".starting-point.inventoryid-" + inventoryId),
			$form = $divStructure.find("form[data-inventory-id='" + inventoryId + "'][data-config-id='" + configId + "']").first().clone().addClass("creating"),
			replacingClass = "replacing";
        //                console.log(inventoryId);
        //                console.log($form);

        // checking one is added before but not saved.
        var previousFormCreatedCount = $("form.creating[data-inventory-id='" + inventoryId + "'][data-config-id='" + configId + "']").length;
        if (previousFormCreatedCount > 0) {
            return false;
        }

        // making form elements enhanable for jquery Mobile.
        var changableElements = $form.find(".needsChange").removeClass("needsChange");
        // console.log(changableElements);
        for (var i = 0; i < changableElements.length; i++) {
            var element = $(changableElements[i]);
            var newRole = element.attr("data-new-role");
            if ($.isEmptyObject(newRole)) {
                // remove data-role
                element.removeAttr("data-new-role");
                element.removeAttr("data-role");
            } else {
                // replace the data-new-role to data-role
                element.attr("data-role", newRole);
                element.removeAttr("data-new-role");
            }
            // console.log(element);
        }

        //add the form to the adding place.
        $form.appendTo($divStartingPoint.find(".addObjects-here"));
        // enhance jquery mobile.
        $('[data-role="page"]').trigger('create');

    });



});