import * as XLSX from 'xlsx';

function DownloadXlsxButton({test, questions, version}) {
    const separator = ',';
    
    const downloadXlsx = async () => {
        const response = await getCompletions(test.id, version);
        if (response.ok) {
            const data = await response.json();
            const completions = data.completions;
            
            console.log(completions);
            
            let table = [];
            
            // Header
            let headerRow = ["Пользователь", "Начат", "Пройден", "Количество верных ответов", "Процент верных ответов"];
            for (let i = 0; i < questions.length; i++) {
                headerRow.push(cleanText(questions[i].description));
            }

            table.push(headerRow);
            
            // Parse completions
            completions.forEach(completion => {
                let row = [];
                if (completion.userId === null) {
                    row.push("Аноним");
                }
                else {
                    row.push(completion.user.username);
                }
                row.push(completion.startedAt, completion.completedAt, completion.correctAnswers.length, completion.completionPercentage);
                
                // Answer data
                for (let i = 0; i < questions.length; i++) {
                    const correspondingAnswer = completion.answers.find(a => a.questionId === questions[i].id);
                    const isCorrect = (completion.correctAnswers.find(a => a.id === correspondingAnswer.id));
                    
                    let answerString;
                    
                    switch (questions[i].type) {
                        // Single and multiple choice
                        case 0:
                        case 1:
                            const options = questions[i].data.options.filter((o, i) => correspondingAnswer.answer.chosenIndices.includes(i));
                            answerString = options.map(o => cleanText(o)).join("; ");
                            break;
                        // Number input
                        case 2:
                            answerString = cleanText(correspondingAnswer.answer.numberAnswer);
                            break;
                        // Text input
                        case 3:
                            answerString = cleanText(correspondingAnswer.answer.textAnswer);
                            break;
                        // Sequence
                        case 4:
                            answerString = correspondingAnswer.answer.sequence.map(o => cleanText(o)).join("; ");
                            break;
                    }
                    
                    if (isCorrect) {
                        answerString += ' ✔';
                    }
                    else {
                        answerString += ' ✖';
                    }
                    row.push(answerString);
                }
                table.push(row);
            });
            
            const worksheet = XLSX.utils.aoa_to_sheet(table);
            const workbook = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(workbook, worksheet, "Статистика");
            
            const currentDate = new Date();
            const dateString = currentDate.toLocaleDateString().replaceAll('.', '-');
            const timeString = currentDate.toLocaleTimeString().replaceAll(':', '-');
            const fileName = `${test.name}_stats_${dateString}_${timeString}.xlsx`;

            console.log('yea');
            XLSX.writeFile(workbook, fileName);
        }
    }
    
    return (<button className="btn btn-primary" onClick={downloadXlsx}>Скачать таблицу в формате Excel</button>)
}

function cleanText(text) {
    // Clean text to remove multiple spaces and jumpline (break csv)
    let data = text.replace(/(\r\n|\n|\r)/gm, '').replace(/\s+/g, ' ');
    // Escape double-quote with double-double-quote
    data = data.replace(/"/g, '""');
    // Clean BBCode markup
    data = data.replace(/\[(\/)?(?:b|i|img|center|u|color|s|quote|url|ul|ol|li|table|tr|td|th)(?:=[^\]]*)?\]/gi, '');
    // Trim if the string is too long
    if (data.length > 32) {
        data = data.substring(0, 32);
    }
    
    return data;
}

function getCompletions(id, version) {
    return fetch(`/api/tests/${id}/completions/finished?version=${version}`);
}

export default DownloadXlsxButton;