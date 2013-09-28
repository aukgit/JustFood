/// <reference path="jquery-2.0.3.js" />
/// <reference path="jquery-2.0.3-vsdoc.js" />
/// <reference path="jquery.mobile-1.3.2.js" />

$(function () {

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

    $(".clickedButtonOnInventoryOut,ui-down").click(function (e) {
        var $this = $(this);
        //e.preventDefault();
        console.log($this.attr('class'));

    });

    $(".execute_dropdown").change(function (e) {
        var inventoryID = $(this).attr("data-inventory"),
            typeOfInventoryForm = $(this).attr("data-inventory-type"),
            $form = $("form.inventoryid-" + inventoryID + "." + typeOfInventoryForm),
            replacingClass = "replacing";





        $.ajax({
            data: $form.serialize(),
            type: $form.attr("method"),
            url: $form.attr("action"),

            beforeSend: function () {
                var $this = $(this),
                            theme = "j" || $.mobile.loader.prototype.options.theme,
                            msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
                            textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
                            textonly = !!$this.jqmData("textonly");
                html = $this.jqmData("html") || "";
                $.mobile.loading("show", {
                    text: "Saving",
                    textVisible: true,
                    theme: theme,
                    textonly: textonly,
                    html: html
                });
            },

            success: function () {
                var savingTypeForm = "saved";
                var $this = $(this),
                            theme = "j" || $.mobile.loader.prototype.options.theme,
                            msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
                            textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
                            textonly = !!$this.jqmData("textonly");
                html = $this.jqmData("html") || "";
                $.mobile.loading("show", {
                    text: "Saved",
                    textVisible: true,
                    theme: theme,
                    textonly: textonly,
                    html: html
                });
                var hideLoading = function () {
                    $.mobile.loading("hide");
                };

                if ($form.hasClass("leftover")) {
                    //adding form
                    var $divStructure = $(".structure hide").clone().removeClass("hide"),
                        $prevHiddenInputCreator = $form.filter(".Hidden-Inputs-Creator").html(),
                        $savingForm = $("form.saved").first(),
                        newFromString = "<form class='Creating-New-Form' action='" + $savingForm.attr("action") + "' method='post'></form>",
                        $divStartingPoint = $(".starting-point.inventoryid-" + inventoryID),
                        $divStartingPointClone = $divStartingPoint.clone(),
                        $newHiddenInputCreator = $divStartingPointClone.filter(".Hidden-Inputs-Creator"),
                        $newFormCreation = $divStartingPoint.prependTo(newFromString),
                        $newForm = $(".Creating-New-Form").first().removeClass("Creating-New-Form");

                    $prevHiddenInputCreator.appendTo($newHiddenInputCreator);

                }


                JqueryWaitFunction(300).done(hideLoading);
            },

            error: function () {
                var $this = $(this),
                            theme = "e" || $.mobile.loader.prototype.options.theme,
                            msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
                            textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
                            textonly = !!$this.jqmData("textonly");
                html = $this.jqmData("html") || "";
                $.mobile.loading("show", {
                    text: "Failed",
                    textVisible: true,
                    theme: theme,
                    textonly: textonly,
                    html: html
                });
                var hideLoading = function () {
                    $.mobile.loading("hide");
                };
                JqueryWaitFunction(600).done(hideLoading);

            }
        });
    });
});