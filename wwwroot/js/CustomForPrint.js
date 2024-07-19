$(document).ready(function (){
    $('.exportBtn').click(function(e){
        //$('.exportBtn').hide();
        e.preventDefault();
        e.stopPropagation();
        $('#dynamicTable').printThis({
            debug: false,                       // show the iframe for debugging
            importCSS: true,                    // import parent page css
            importStyle: true,                  // import style tags
            printContainer: true,               // print outer container/$.selector
            loadCSS: "",                        // path to additional css file - use an array [] for multiple
            pageTitle: "",                      // add title to print page
            removeInline: true,                // remove inline styles from print elements
            removeInlineSelector: "*",          // custom selectors to filter inline styles. removeInline must be true
            printDelay: 1000,                   // variable print delay
            header: null,                       // prefix to html
            footer: null,                       // postfix to html
            base: true,                        // preserve the BASE tag or accept a string for the URL
            formValues: true,                   // preserve input/form values
            canvas: true,                       // copy canvas content
            doctypeString: '<!DOCTYPE html>',   // enter a different doctype for older markup
            removeScripts: true,               // remove script tags from print content
            copyTagClasses: true,               // copy classes from the html & body tag
            copyTagStyles: true,                // copy styles from html & body tag (for CSS Variables)
            beforePrintEvent: null,             // callback function for printEvent in iframe
            beforePrint: null,                  // function called before iframe is filled
            afterPrint: null                    // function called before iframe is removed
        });
        //$('.exportBtn').show();
    });
});


$(document).ready(function () {
    $('#exportParkingI').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#ParkingGridId').printThis({
            debug: false,                       // show the iframe for debugging
            importCSS: true,                    // import parent page css
            importStyle: true,                  // import style tags
            printContainer: true,               // print outer container/$.selector
            loadCSS: "",                        // path to additional css file - use an array [] for multiple
            pageTitle: "",                      // add title to print page
            removeInline: true,                // remove inline styles from print elements
            removeInlineSelector: "*",          // custom selectors to filter inline styles. removeInline must be true
            printDelay: 1000,                   // variable print delay
            header: null,                       // prefix to html
            footer: null,                       // postfix to html
            base: true,                        // preserve the BASE tag or accept a string for the URL
            formValues: true,                   // preserve input/form values
            canvas: true,                       // copy canvas content
            doctypeString: '<!DOCTYPE html>',   // enter a different doctype for older markup
            removeScripts: true,               // remove script tags from print content
            copyTagClasses: true,               // copy classes from the html & body tag
            copyTagStyles: true,                // copy styles from html & body tag (for CSS Variables)
            beforePrintEvent: null,             // callback function for printEvent in iframe
            beforePrint: null,                  // function called before iframe is filled
            afterPrint: null                    // function called before iframe is removed
        });
        //$('.exportBtn').show();
    });
});$(document).ready(function () {
    $('#exportId').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#dynamicTable').printThis({
            debug: false,                       // show the iframe for debugging
            importCSS: true,                    // import parent page css
            importStyle: true,                  // import style tags
            printContainer: true,               // print outer container/$.selector
            loadCSS: "",                        // path to additional css file - use an array [] for multiple
            pageTitle: "",                      // add title to print page
            removeInline: true,                // remove inline styles from print elements
            removeInlineSelector: "*",          // custom selectors to filter inline styles. removeInline must be true
            printDelay: 1000,                   // variable print delay
            header: null,                       // prefix to html
            footer: null,                       // postfix to html
            base: true,                        // preserve the BASE tag or accept a string for the URL
            formValues: true,                   // preserve input/form values
            canvas: true,                       // copy canvas content
            doctypeString: '<!DOCTYPE html>',   // enter a different doctype for older markup
            removeScripts: true,               // remove script tags from print content
            copyTagClasses: true,               // copy classes from the html & body tag
            copyTagStyles: true,                // copy styles from html & body tag (for CSS Variables)
            beforePrintEvent: null,             // callback function for printEvent in iframe
            beforePrint: null,                  // function called before iframe is filled
            afterPrint: null                    // function called before iframe is removed
        });
        //$('.exportBtn').show();
    });
});