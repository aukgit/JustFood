/// <reference path="jquery-2.0.3.js" />
/// <reference path="jquery-2.0.3.min.map" />
/// <reference path="jquery-2.0.3-vsdoc.js" />
/// <reference path="_references.js" />
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

    //add config button
    $(".Inventory-Out-Config-Page a.AddConfig").click(function (e) {
        e.preventDefault();
        var $this = $(this),
            inventoryId = $this.attr("data-inventory-id"),
            $divStructure = $("div.structure"),
            $divStartingPoint = $(".starting-point.inventoryid-" + inventoryId),
            $form = $divStructure.find("form[data-inventory-id='" + inventoryId + "']").clone(),
            replacingClass = "replacing";
        console.log(inventoryId);
        console.log($form);

        //making form elements enhanable for jquery Mobile.
        var changableElements = $form.find(".needsChange").removeClass("needsChange");
        console.log(changableElements);
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
            console.log(element);
        }
        $form.appendTo($divStartingPoint);
        //enhance jquery mobile.
        $('[data-role="page"').trigger('create');

    });



});