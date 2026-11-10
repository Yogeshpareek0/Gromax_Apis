using GromaxMobileApis.Models.Services;
using System.Net;
using System.Text;

public static class EmailTemplateHelper
{
    public static string GenerateItemScoreEmailBody(ItemScoreEmailModel model)
    {
        string dealerName = WebUtility.HtmlEncode(model.DealerName);
        string assessmentMonth = WebUtility.HtmlEncode(model.AssessmentMonth);
        string category = WebUtility.HtmlEncode(model.DealerCategory);

        string categoryMessage = model.DealerCategory?.ToUpper() == "A"
            ? @"
                <p><b>Congratulations!</b> Your dealership has successfully
                achieved A Category Status.</p>

                <p>Your commitment towards maintaining dealership infrastructure,
                service tools, workshop environment, and skilled manpower reflects
                your dedication to delivering quality service and enhancing
                customer satisfaction.</p>

                <p>We appreciate your continued efforts and encourage you to
                sustain these standards.</p>
            "
            : @"
                <p>Based on the current assessment, your dealership has been
                categorized as B Category.</p>

                <p>We request you to review the identified improvement areas
                and take the necessary corrective actions to upgrade your
                facilities and operational standards.</p>

                <p>Our team will be happy to support you in achieving
                A Category status in the upcoming assessment cycle.</p>
            ";

        StringBuilder body = new StringBuilder();

        body.Append($@"
        <html>
        <body style='font-family:Arial,sans-serif;font-size:14px;color:#333;'>

            <p>Dear <b>{dealerName}</b>,</p>

            <p>Greetings from Gromax Agri Equipment Ltd.</p>

            <p>
                We are pleased to share the results of your dealership's
                ITEM (Infrastructure, Tools, Environment & Manpower)
                Assessment conducted on
                <b>{model.AssessmentDate:dd-MMM-yyyy}</b>.
            </p>

            <p>
                The ITEM assessment is designed to evaluate dealership
                readiness and service capability across key operational
                parameters, ensuring a consistent and superior customer experience.
            </p>

            <h3>Assessment Summary - {assessmentMonth}</h3>

            <table style='border-collapse:collapse;width:100%;'
                   border='1' cellpadding='10'>

                <tr style='background-color:#eeeeee;'>
                    <th align='left'>Parameter</th>
                    <th>Maximum Score</th>
                    <th>Achieved Score</th>
                </tr>

                <tr>
                    <td>Infrastructure (I)</td>
                    <td align='center'>180</td>
                    <td align='center'>{model.InfrastructureScore}</td>
                </tr>

                <tr>
                    <td>Tools (T)</td>
                    <td align='center'>150</td>
                    <td align='center'>{model.ToolsScore}</td>
                </tr>

                <tr>
                    <td>Environment (E)</td>
                    <td align='center'>150</td>
                    <td align='center'>{model.EnvironmentScore}</td>
                </tr>

                <tr>
                    <td>Manpower (M)</td>
                    <td align='center'>120</td>
                    <td align='center'>{model.ManpowerScore}</td>
                </tr>

                <tr style='background-color:#eeeeee;font-weight:bold;'>
                    <td>Total Score</td>
                    <td align='center'>600</td>
                    <td align='center'>{model.TotalScore}</td>
                </tr>

            </table>

            <h3>Overall Performance</h3>

            <p>
                <b>ITEM Score Achieved:</b> {model.ScorePercentage:0.00}%
            </p>

            <p>
                <b>Dealer Category:</b> {category}
            </p>

            <h3>Dealer Categorization Criteria</h3>

            <p>A Category: ITEM Score 70% and above</p>
            <p>B Category: ITEM Score below 70%</p>

            <h3>Remarks</h3>

            {categoryMessage}

            <p>
                We thank you for your continued partnership and contribution
                towards strengthening the Gromax service network.
            </p>

            <p>
                For any clarification regarding the assessment, please contact
                your respective Area Service Manager.
            </p>

            <br/>

            <p>
                Regards,<br/>
                <b>Gromax Agri Equipment Ltd.</b>
            </p>

        </body>
        </html>
        ");

        return body.ToString();
    }
}