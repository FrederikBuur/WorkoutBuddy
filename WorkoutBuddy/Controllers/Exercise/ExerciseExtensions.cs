namespace workouts.Util
{
    public static class ExerciseExtensions
    {
        public static Exercise ToExercise(this ExerciseDto e) => new Exercise(
            e.Id,
            e.CreaterId,
            e.Name,
            e.Description,
            e.ImageUrl,
            e.MuscleGroups.Select(mg => mg.ToMuscleGroup())
        );

        public static ExerciseDto ToExerciseDto(this Exercise e) => new ExerciseDto
        {
            Id = e.id,
            CreaterId = e.creatorId, 
            Name = e.name,
            Description = e.description,
            ImageUrl = e.imageUrl,
            MuscleGroups= e.muscleGroups.Select(mg => mg.ToMuscleGroupDto())
        };

        public static MuscleGroup ToMuscleGroup(this MuscleGroupDto mg) => new MuscleGroup(
            mg.Id,
            mg.Activation,
            mg.Type
        );

        public static MuscleGroupDto ToMuscleGroupDto(this MuscleGroup mg) => new MuscleGroupDto
        {
            Id = mg.id,
            Type = mg.type,
            Activation = mg.activation,
        };
    }
}
