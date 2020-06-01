# Com'Eyes

Ce projet s’adresse aux enfants souffrant de handicaps moteurs lourds. La plupart de ces enfants peuvent uniquement bouger leurs yeux qui sont alors le seul moyen de communication avec leur entourage et les soignants. Pour répondre à cette problématique, **Humanlab Saint-Pierre** a initié ce projet d'application Windows de suivi oculaire. Elle a été réalisée par Anissa LAMHAOUAR et Inès MISSOUM BENZIANE dans le cadre de leur projet de fin d'études à **Polytech Montpellier**.

## Fonctionnalités
Des « grilles » peuvent être créées par les parents ou le corps médical. Ces grilles sont composées d’« éléments ». Un élément est constitué d’une image et d’un son qui se déclenche quand l’enfant fixe un certain temps l’image. Ce son peut être un fichier son ou un texte prononcé. Différents paramètres peuvent être fixés à la conception de la grille : 

● Le nombre d'éléments  
● La taille des éléments  
● La position des éléments  
● Le temps de fixation nécessaire pour les déclencher 

Ces grilles peuvent ensuite être utilisées dans des « activités ». Une activité est une succession de grilles ordonnées. 

Ainsi, ce système répond à la fois au besoin de création d’exercices ludiques adaptables à tous les enfants et à l’expression des besoins. En effet, il est possible pour les parents de créer des grilles aux thématiques de leurs souhaits afin de permettre à leur enfant de communiquer. Les parents peuvent fixer des contraintes adaptées aux capacités de leur enfant à un moment donné (taille des images, temps de fixation …) et pourront de ce fait adapter les grilles aux progrès de rééducation.

## Technologies

### Capteur
Le **Tobii Eye Tracker 4C** est utilisé pour récupérer la position du regard de l'enfant sur l'écran.
### Technologies de développement
C'est une application **UWP (Universal Windows Application)** développée en **C# et Xaml** suivant le modèle de conception **MVVM**. Elle utilise une base de données **SQLite**.
Nous conseillons d'utiliser **Microsoft Visual Studio** pour éditer le projet.

<div style="text-align:center;">
<img src="Humanlab.jpg" height='150' alt="Logo Humanlab"/>
<img src="Com'Eyes.png" height='100' alt="Logo application"/>
</div>
