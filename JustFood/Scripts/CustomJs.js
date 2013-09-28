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
        var inventoryId = $(this).attr("data-inventory"),
            typeOfInventoryForm = $(this).attr("data-inventory-type"),
            $form = $("form.inventoryid-" + inventoryId + "." + typeOfInventoryForm),
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
                    var $divStructure = $(".starting-point.inventoryid-" + inventoryId + " .structure.hide")
                                                                                                    .first()
                                                                                                    .clone()
                                                                                                    .removeClass("hide"),
                        $prevHiddenInputCreatorHtml = $form.find(".Hidden-Inputs-Creator").clone(),
                        $savingForm = $("form.saved").first(),
                        newFromString = "<form class='Creating-New-Form' action='" + $savingForm.attr("action") + "' method='post'></form>",
                        $divStartingPoint = $(".starting-point.inventoryid-" + inventoryId),
                        $newFormCreation = $divStartingPoint.prepend(newFromString),
                        $newForm = $(".Creating-New-Form").first().removeClass("Creating-New-Form"),
                        $divDiscardSelection = $newForm.find(".discardSelection"),
                        $divQuantityValue = $newForm.find(".discardSelection"),
                        $divQuantityType = $newForm.find(".discardSelection"),
                        $divButtonPlaces = $newForm.find(".buttonPlaces"),
                        $listDiscardItems = $form.find("[name='DiscardCategoryID'").clone(),
                        $textQuantity = $form.find("[name='Quantity'").clone(),
                        
                        ;

                    /** PrependTo/Prepend or Append/AppendTo:
                      * When HTML add use append(bottom in the list) or prepend (top in the list).  
                      * When add jQuery object like clone or any selected object use appendTo or prependTo.
                      * */

                    console.log("$divStructure : ");
                    console.log($divStructure);
                    
                    console.log("$prevHiddenInputCreatorHtml : ");
                    console.log($prevHiddenInputCreatorHtml);
                    console.log("$savingForm : ");
                    console.log($savingForm);
                    console.log("$divStartingPoint : ");
                    console.log($divStartingPoint);

                
                    console.log("$newForm : ");
                    console.log($newForm);
                    

                    // now add the div structure to the new form
                    $divStructure.appendTo($newForm);
                    //add hidden inputs to the structure's new free space for hiddens
                    $prevHiddenInputCreatorHtml.prependTo($newForm);
                    $newForm.prependTo($divStartingPoint);
                    
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