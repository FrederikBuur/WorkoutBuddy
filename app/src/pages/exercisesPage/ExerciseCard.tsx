import "./ExerciseCard.css";
import { ExerciseDetail } from "../../api/exerciseDetailsTypes";
import { Component, For } from "solid-js";

interface ExerciseCardProps {
  exercise: ExerciseDetail;
}

const ExerciseCard: Component<ExerciseCardProps> = (props) => {
  const placeholderImagePath = "src/assets/exercise-placeholder.jpg";

  return (
    <div class="exercise-container">
      <img
        class="exercise-image"
        src={props.exercise?.imageUrl ?? placeholderImagePath}
      />
      <div class="exercise-info-container">
        <h4>{props.exercise.name}</h4>
        <span>{props.exercise.description}</span>
        <div class="exercise-muscle-group-container">
          <For each={props.exercise.muscleGroups.split(",")}>
            {(item, index) => <span class="exercise-muscle-group">{item}</span>}
          </For>
        </div>
      </div>
    </div>
  );
};

export default ExerciseCard;
