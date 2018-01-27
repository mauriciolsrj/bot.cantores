using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace beatriz.bot.cantores.Dialogs
{
    [Serializable]
    public class QnaDialog : QnAMakerDialog
    {
        public QnaDialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionKey"], ConfigurationManager.AppSettings["QnaKnowledgeBaseId"], "Desculpe, ainda não aprendi sobre isso.", 0.5)))
        {

        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var primeiraResposta = result.Answers.First().Answer;

            Activity resposta = ((Activity)context.Activity).CreateReply();

            var dadosResposta = primeiraResposta.Split(';');

            if (dadosResposta.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var titulo = dadosResposta[0];
            var descricao = dadosResposta[1];
            var url = dadosResposta[2];
            var image = dadosResposta[3];  

            HeroCard card = new HeroCard()
            {
                Title = titulo,
                Subtitle = descricao
            };

            card.Buttons = new List<CardAction>()
            {
              new CardAction(ActionTypes.OpenUrl, "Ver o Instagram", value: url)
            };

            card.Images = new List<CardImage>()
            {
                 new CardImage(url = image)
        };

            resposta.Attachments.Add(card.ToAttachment());

            await context.PostAsync(resposta);

        }
    }
}