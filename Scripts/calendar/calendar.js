

const currentYear = new Date().getFullYear();

new Calendar('#calendar', {
    dataSource: [
        {
            startDate: new Date(currentYear, 2, 5),
            endDate: new Date(currentYear, 2, 20)
        },
        {
            startDate: new Date(currentYear, 5, 10),
            endDate: new Date(currentYear, 7, 10)
        }
    ],
    language: 'pt'
})



$(document).ready(function () {

});