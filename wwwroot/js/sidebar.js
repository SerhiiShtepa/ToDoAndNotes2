$(function () {
    // Sidebar toggle behavior
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar, #content').toggleClass('active');
    });
});

// .toggleClass('active') method enables CSS class 'active' for elements ('#sidebar, #content').
// If the active class is already present, it will be removed, and if it is not, it will be added.

function auto_grow(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight) + "px";
}
